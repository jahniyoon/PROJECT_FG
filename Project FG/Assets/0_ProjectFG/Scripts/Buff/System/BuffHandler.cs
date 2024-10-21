using JH;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace JH
{

    public class BuffHandler : MonoBehaviour
    {
        [Header("버프 상태")]
        [SerializeField] private BuffStatus m_status;
        [Header("영향을 받고있는 버프 리스트")]
        [SerializeField] private BuffElementList m_buffs = new BuffElementList();   // 영향받고있는 버프 리스트
        [Header("하나만 적용되어야하는 버프 리스트")]
        [SerializeField] private BuffElementList m_overlapBuffs = new BuffElementList();    // 중첩이되어 계산이 필요한 버프들

        [Header("기본 버프")]

        [SerializeField] private BuffBase[] m_inherenceBuff;

        #region Property
        public BuffStatus Status => m_status;
        #endregion

        [Header("Buff Event")]
        [HideInInspector] public UnityEvent AddBuffEvent;
        [HideInInspector] public UnityEvent RemoveBuffEvent;

        WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
        private void Awake()
        {
            Init();
        }


        private void Update()
        {
            // 타이머를 업데이트한다.
            m_status.UpdateTimer(Time.deltaTime);
        }



        // 버프 초기화
        public void Init()
        {
            RemoveAllBuff();
            // 상태이상 초기화

            m_status.Init(this);

            // 장착된 버프들을 활성화한다.
            foreach (var buff in m_inherenceBuff)
            {
                int casterID = GFunc.XORCombine(this.gameObject.GetInstanceID(), buff.ID);
                AddBuff(casterID, buff);
            }
        }

        // 버프를 받는 부분 (파스 붙이는 부분)
        public void OnBuff(GameObject caster, BuffBase buff)
        {
            // 키가 중복되지 않도록 하기
            int casterID = GFunc.XORCombine(caster.GetInstanceID(), buff.ID);

            // 버프가 없는 경우에만 추가
            if (m_buffs.Contains(casterID) == false)
                AddBuff(casterID, buff);

            // 새로운 버프가 추가되면 계산을 한번 해준다.
            CalculateBuff();
        }

        // 버프를 추가한다.
        private void AddBuff(int casterID, BuffBase buff)
        {
            AddBuffEvent?.Invoke();

            // 버프 세팅하고
            BuffElement newBuff = new BuffElement();
            newBuff.SetBuff(casterID, buff);

            // 리스트에 버프 추가
            m_buffs.Add(newBuff);
        }



        // 버프를 제거한다.
        public void RemoveBuff(int casterID, BuffBase buff)
        {
            RemoveBuffEvent?.Invoke();

            // 삭제할 버프가 리스트에 없으면 패스
            if (m_buffs.Contains(casterID) == false)
                return;

            // 버프 리스트에서 삭제. 만약 활성화 되어있는 버프면 비활성화를 한다.
            InactiveBuff(m_buffs.Buff(casterID));
            m_buffs.Remove(m_buffs.Buff(casterID));

            //버프가 제거되면 계산을 한번 해준다.
            CalculateBuff();
        }

        // 버프를 제거한다.
        // isMaintain 유지가 되어야한다.
        public void RemoveBuff(GameObject caster, BuffBase buff)
        {
            int casterID = GFunc.XORCombine(caster.GetInstanceID(), buff.ID);
            RemoveBuff(casterID, buff);
        }

        // 버프 활성화
        public void ActiveBuff(BuffElement buffElement)
        {
            // 이미 켜져있으면 또 킬필요 없음
            if (buffElement.isActive)
                return;

            buffElement.Buff.ActiveBuff(this);
            buffElement.SetActive(true);

            // 지속 시간 버프인 경우에만 코루틴을 시작
            if (buffElement.Buff.Data.Condition == BuffEffectCondition.Duration)
                buffElement.Coroutine = StartCoroutine(DurationBuffRoutine(buffElement.CasterID, buffElement));
        }

        //  지속 시간 이후 버프를 해제해준다.
        IEnumerator DurationBuffRoutine(int casterID, BuffElement buffElement)
        {
            BuffBase buff = buffElement.Buff;
            float timer = 0;
            while (timer <= buff.GetDuration())
            {
                timer += Time.deltaTime;
                yield return null;
            }

            RemoveBuff(casterID, buff);
            yield break;
        }


        // 활성화할 버프를 계산한다.
        private void CalculateBuff()
        {
            ClearOverlapBuff();


            foreach (var targetBuff in m_buffs.BuffList)
            {
                // 오버랩할 필요가 없으면 바로 실행
                if(targetBuff.Buff.Data.OverlapType != BuffOverlap.SameTypeHighValue)
                {
                    ActiveBuff(targetBuff);
                    continue;
                }


                // 겹치는 버프가 있으면 겹친다.
                if (CheckOverlapType(targetBuff, out BuffElement overlapBuff))
                    OverlapBuff(overlapBuff, targetBuff);

                // 없으면 추가한다.
                else                
                AddOverlapBuff(targetBuff);
                
            }
        }

        private void ClearOverlapBuff()
        {
            foreach (var buff in m_overlapBuffs.BuffList)
            {
                buff.SetStack(0);
                InactiveBuff(buff);
            }
            m_overlapBuffs.Clear();
        }
        // 버프를 겹쳐준다.
        private void OverlapBuff(BuffElement overlapBuff, BuffElement targetBuff)
        {
            m_overlapBuffs.Remove(overlapBuff);

            // 기존 버프를 해제한 뒤에 비교한 뒤 다시 추가
            InactiveBuff(overlapBuff);
            overlapBuff.StackUp();
            overlapBuff.ComparisonBuff(targetBuff.Buff);

            m_overlapBuffs.Add(overlapBuff);

            ActiveBuff(overlapBuff);
        }

        // 버프를 중첩시킨다.
        private void AddOverlapBuff(BuffElement buff)
        {
            m_overlapBuffs.Add(buff);
            ActiveBuff(buff);
        }

        // 겹쳐야하는 버프가 있는지 체크한다.
        private bool CheckOverlapType(BuffElement target, out BuffElement overlapBuff)
        {
            overlapBuff = null;

            foreach (var buff in m_overlapBuffs.BuffList)
            {
                if (buff.Buff.Type != target.Buff.Type)
                    continue;

                // 조건도 비교한다.
                if (buff.Buff.Data.Condition != target.Buff.Data.Condition)
                    continue;

                overlapBuff = buff;
                return true;
            }
            return m_overlapBuffs.Contains(target.CasterID);
        }

 


        // 버프들은 상태 이상에 수정할 정보를 전달해준다.
        public void BuffStatus(BuffStatus newStatus)
        {
            m_status.Stun(newStatus);
        }


        // 버프 비활성화
        private void InactiveBuff(BuffElement buffElement)
        {
            // 활성화 되었을 때만 끈다
            if (buffElement.isActive)
            {
                buffElement.Buff.InactiveBuff(this);
                buffElement.SetActive(false);
            }
            if (buffElement.Coroutine != null)
                StopCoroutine(buffElement.Coroutine);
        }


        public void RemoveAllBuff()
        {
            StopAllCoroutines();

            // 버프 리스트 내의 모든 버프를 해제한다.
            for (int i = m_buffs.BuffList.Count - 1; 0 <= i; i--)
            {
                RemoveBuff(m_buffs.BuffList[i].CasterID, m_buffs.BuffList[i].Buff);
            }
        }







        #region Legacy


        //// 버프의 활성화를 체크한다.
        //public bool BuffEnableCheck(GameObject caster, BuffData buffElement)
        //{
        //    int casterID = GFunc.XORCombine(caster.GetInstanceID(), buffElement.GetInstanceID());
        //    return m_buffs.ContainsKey(casterID);
        //}

        //// 버프 ID기준, 같은 계열의 버프가 이미 켜져있는지 확인한다.
        //private int CanMainBuff(BuffElement TargetBuff)
        //{
        //    foreach (var buffElement in m_buffs)
        //    {
        //        if (TargetBuff.Stun.CasterID != buffElement.Value.Stun.CasterID)
        //            continue;

        //        // 돌아가는게 있을 때
        //        if (buffElement.Value.isOneAndOnlyEnable)
        //        {
        //            // 우선도가 높거나 같으면 계승받는다.
        //            if (buffElement.Value.Stun.Priority <= TargetBuff.Stun.Priority)
        //            {
        //                // 코루틴을 꺼준다.
        //                buffElement.Value.isOneAndOnlyEnable = false;
        //                InactiveBuff(buffElement.Value);
        //                return buffElement.Key;
        //            }
        //            else
        //                return 0;
        //        }
        //    }
        //    return TargetBuff.CasterID;
        //}


        ////TODO : 여기 버프 종류별로 다 바꿔야함.
        ///// <summary>
        ///// 한가지 버프 상태만 가져야하는 버프의 경우 버프 체크를 진행한다.
        ///// 가장 높은 우선도를 체크하도록 한다.
        ///// </summary>
        ///// <param Name="buffElement">체크할 버프</param>
        ///// <param Name="checkThis">체크할 버프를 함께 검사할지 정한다. 삭제할때는 필요가 없다.</param>
        //public void OnlyOneBuffCheck(BuffBase buffElement, int checkThis = default)
        //{
        //    // 기준을 정한다.
        //    BuffElement priorityBuff = null;

        //    foreach (var item in m_buffs)
        //    {

        //        // 같은 종류의 버프인지 체크한다.
        //        if (item.Value.Stun.CasterID != buffElement.CasterID)
        //            continue;

        //        // 같은 버프 중 켜져있다면 일단 다 꺼준다.
        //        if (item.Value.isActive)
        //        {
        //            item.Value.Stun.InactiveBuff(this);
        //            item.Value.SetActive(false);
        //        }
        //        if (item.Key == checkThis)
        //            continue;


        //        // 만약 비어있으면 넣는다.
        //        if (priorityBuff == null)
        //        {
        //            priorityBuff = item.Value;
        //            continue;
        //        }

        //        if (priorityBuff.Stun.Priority < item.Value.Stun.Priority)
        //            priorityBuff = item.Value;
        //    }
        //    if (priorityBuff == null)
        //        return;

        //    // 선정된 버프를 활성화해준다.
        //    InactiveBuff(priorityBuff.Coroutine);
        //    priorityBuff.Coroutine = StartCoroutine(DurationBuffRoutine(priorityBuff.CasterID, priorityBuff));

        //}

        //// 유지할 버프가 있는지 체크하고 유지한다.
        //private bool MaintainBuffCheck(BuffElement buffElement, int checkThis = default)
        //{
        //    BuffBase buffElement = buffElement.Stun;

        //    // 기준을 정한다.
        //    BuffElement priorityBuff = null;

        //    foreach (var item in m_buffs)
        //    {

        //        // 같은 종류의 버프인지 체크한다.
        //        if (item.Value.Stun.CasterID != buffElement.CasterID)
        //            continue;

        //        // 같은 버프 중 켜져있다면 일단 다 꺼준다.
        //        if (item.Value.isActive)
        //        {
        //            item.Value.Stun.InactiveBuff(this);
        //            item.Value.SetActive(false);
        //        }
        //        if (item.Key == checkThis)
        //            continue;


        //        // 만약 비어있으면 넣는다.
        //        if (priorityBuff == null)
        //        {
        //            priorityBuff = item.Value;
        //            continue;
        //        }

        //        if (priorityBuff.Stun.Priority < item.Value.Stun.Priority)
        //            priorityBuff = item.Value;
        //    }
        //    if (priorityBuff == null)
        //        return false;

        //    // 계승한다.
        //    priorityBuff.MainTain(buffElement);


        //    // 선정된 버프를 활성화해준다.
        //    InactiveBuff(buffElement.Coroutine);
        //    InactiveBuff(priorityBuff.Coroutine);

        //    priorityBuff.Coroutine = StartCoroutine(DurationBuffRoutine(priorityBuff.CasterID, priorityBuff));
        //    return true;
        //}

        //public void ResetStack(BuffBase buffElement)
        //{
        //    foreach (var item in m_buffs)
        //    {
        //        if (item.Value.Stun.CasterID == buffElement.CasterID)
        //        {
        //            item.Value.ResetStack();
        //        }
        //    }

        //}

        // 버프들은 상태 이상에 수정할 정보를 전달해준다.
        //public void DeBuffStatus(BuffStatus newStatus)
        //{
        //    m_status.DeBuff(newStatus);
        //}
        #endregion Legacy
    }
}

