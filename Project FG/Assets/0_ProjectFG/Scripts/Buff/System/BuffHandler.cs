using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JH
{

    public class BuffHandler : MonoBehaviour
    {
        [Header("버프 상태")]
        [SerializeField] private Status m_status;
        [SerializeField] private SerializableDictionary<int, BuffElement> m_buffList = new SerializableDictionary<int, BuffElement>();

        [Header("기본 버프")]

        [SerializeField] private BuffBase[] m_inherenceBuff;

        #region Property
        public Status Status => m_status;
        #endregion

        [Header("Buff Event")]
        [HideInInspector] public UnityEvent AddBuffEvent;
        [HideInInspector] public UnityEvent RemoveBuffEvent;

        WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
        private void Awake()
        {
            Init();

        }

        private void Start()
        {
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
            m_status.Init();


            // 장착된 버프들을 활성화한다.
            foreach (var buff in m_inherenceBuff)
            {
                int casterID = GFunc.XORCombine(this.gameObject.GetInstanceID(), buff.GetInstanceID());
                AddBuff(casterID, buff);
            }
        }

        // 버프를 받는 부분 (파스 붙이는 부분)
        public void OnBuff(GameObject caster, BuffBase buff)
        {
            // 키가 중복되지 않도록 하기
            int casterID = GFunc.XORCombine(caster.GetInstanceID(), buff.GetInstanceID());

            // 버프가 이미 있으면 스택을 쌓아준다.
            if (m_buffList.ContainsKey(casterID))
            {
                if (buff.StackUpTime <= m_buffList[casterID].Timer && m_buffList[casterID].Buff.Type == BuffType.Stack)
                {
                    m_buffList[casterID].Buff.StackBuff(this);
                    m_buffList[casterID].StackUp();
                }
                m_buffList[casterID].isStay = true;
                return;
            }


            // 버프를 추가한다.
            AddBuff(casterID, buff);
        }

        // 버프를 추가한다.
        private void AddBuff(int casterID, BuffBase buff)
        {
            AddBuffEvent?.Invoke();

            BuffElement newBuff = new BuffElement();
            newBuff.ID = casterID;
            newBuff.Buff = buff;
            newBuff.isStay = true;

            // 리스트에 버프 추가하고
            m_buffList.Add(casterID, newBuff);

            // 원앤온리 버프, 이미 돌고있는 버프가 있으면 계승하고 꺼준다.
            if (newBuff.Buff.IsOneAndOnly)
            {
                int buffID = CanMainBuff(newBuff);

                // 우선도가 낮으므로 계승받지도 않고 코루틴을 시작하지도 않는다.
                if (buffID == 0)
                    return;

                // 계승 (자기자신이면 패스나 마찬가지)
                if (m_buffList[buffID].isActive)
                {
                    m_buffList[buffID].Buff.InactiveBuff(this);
                    m_buffList[buffID].SetActive(false);
                }

                m_buffList[casterID].MainTain(m_buffList[buffID]);
                m_buffList[casterID].isOneAndOnlyEnable = true;
            }

            // 코루틴을 시작
            newBuff.Coroutine = StartCoroutine(BuffRoutine(casterID, newBuff));
        }

        // 버프 ID기준, 같은 계열의 버프가 이미 켜져있는지 확인한다.
        private int CanMainBuff(BuffElement TargetBuff)
        {
            foreach (var buff in m_buffList)
            {
                if (TargetBuff.Buff.ID != buff.Value.Buff.ID)
                    continue;

                // 돌아가는게 있을 때
                if (buff.Value.isOneAndOnlyEnable)
                {
                    // 우선도가 높거나 같으면 계승받는다.
                    if (buff.Value.Buff.Priority <= TargetBuff.Buff.Priority)
                    {
                        // 코루틴을 꺼준다.
                        buff.Value.isOneAndOnlyEnable = false;
                        StopCoroutine(buff.Value);
                        return buff.Key;
                    }
                    else
                        return 0;
                }
            }
            return TargetBuff.ID;
        }


        // 버프를 제거한다.
        public void RemoveBuff(int casterID, BuffBase buff, bool isMaintain = false)
        {
            RemoveBuffEvent?.Invoke();

            // 삭제할 버프가 리스트에 없으면 패스
            if (m_buffList.ContainsKey(casterID) == false)
                return;


            // 계승해야하는 원앤온리라면 차기 자신과 같은 ID를 찾는다.
            if (m_buffList[casterID].isOneAndOnlyEnable)
            {
                bool canMainTain = false;
                int PriorityBuffID = 0;

                foreach (var item in m_buffList)
                {
                    // 같은 ID만 체크
                    if (item.Value.Buff.ID != buff.ID)
                        continue;
                    // 자기 자신이면 패스
                    if (item.Key == casterID)
                        continue;

                    // 계승이 가능하다.
                    if (PriorityBuffID == 0)
                    {
                        canMainTain = true;
                        PriorityBuffID = item.Key;
                    }

                    // 우선도를 체크한다.
                    if (m_buffList[PriorityBuffID].Buff.Priority <= item.Value.Buff.Priority)
                        PriorityBuffID = item.Key;

                }

                if (canMainTain)
                {
                    // 켜져있으면 미리 꺼준다.
                    if (m_buffList[casterID].isActive)
                    {
                        m_buffList[casterID].Buff.InactiveBuff(this);
                        m_buffList[casterID].SetActive(false);
                    }

                    // 계승한다.
                    m_buffList[PriorityBuffID].isOneAndOnlyEnable = true;
                    m_buffList[PriorityBuffID].MainTain(m_buffList[casterID]);
                    m_buffList[PriorityBuffID].Coroutine = StartCoroutine(BuffRoutine(PriorityBuffID, m_buffList[PriorityBuffID]));

                }
            }
            else
            {
                if (isMaintain)
                    return;
            }


            StopCoroutine(m_buffList[casterID]);

            // 삭제
            m_buffList.Remove(casterID);



        }
        // 버프를 제거한다.
        // isMaintain 유지가 되어야한다.
        public void RemoveBuff(GameObject caster, BuffBase buff, bool isMaintain = false)
        {
            int casterID = GFunc.XORCombine(caster.GetInstanceID(), buff.GetInstanceID());
            RemoveBuff(casterID, buff, isMaintain);
        }

        public void ActiveBuff(BuffElement buffElement)
        {
            // 이미 켜져있으면 또 킬필요 없음
            if (buffElement.isActive)
                return;

            buffElement.Buff.ActiveBuff(this);
            buffElement.SetActive(true);

            //OnlyOneBuffCheck(buffElement.Buff, buffElement.ID);
        }

        // 받아온 버프를 실행한다.
        IEnumerator BuffRoutine(int casterID, BuffElement buffElement)
        {
            BuffBase buff = buffElement.Buff;
            buffElement.Timer += Time.deltaTime;

            // 타입에 따라 루틴이 달라진다.
            switch (buff.Type)
            {
                // 1. 즉시 효과형
                case BuffType.Immediately:
                    {
                        // 즉시 실행하고 지속시간을 기다린다.
                        ActiveBuff(buffElement);
                        while (buffElement.Timer < buff.Duration)
                        {
                            yield return WaitForFixedUpdate;
                            buffElement.Timer += Time.deltaTime;
                        }
                        break;
                    }

                //2. 조건 확인형 : 지속시간동안 조건을 충족하는지 기다린다.
                case BuffType.TimeCondition:
                    {
                        // 지속시간동안 유지한다.
                        while (0 < buffElement.Timer)
                        {
                            yield return WaitForFixedUpdate;

                            // 조건식이 맞으면 조건식 버프가 실행된다.
                            if (buff.CanActive(buffElement.Timer))
                            {
                                ActiveBuff(buffElement);
                                buffElement.SetActive(false);
                                buffElement.Timer = 0;    // 액티브 타이머는 리셋해준다.
                            }

                            buffElement.UpdateTimer();
                        }
                        break;
                    }
                // 3. 스택형
                case BuffType.Stack:
                    {
                        // 스택이 쌓이는 방식일 경우
                        while (0 <= buffElement.Stack)
                        {
                            yield return WaitForFixedUpdate;

                            if (buffElement.Timer <= 0 && buffElement.isStay == false)
                            {
                                buffElement.StackDown();
                                buffElement.Timer = buff.DecreaseTime;
                            }

                            if (buff.CanActive(buffElement.Stack))
                            {
                                ActiveBuff(buffElement);
                                ResetStack(buffElement.Buff);
                            }


                            buffElement.UpdateTimer();
                        }
                        buffElement.Stack = 0;
                        break;
                    }
                // 버프를 계속 받는동안 계속 유지된다.
                case BuffType.Stay:
                    {
                        ActiveBuff(buffElement);
                        while (0 < buffElement.Timer)
                        {
                            yield return WaitForFixedUpdate;
                            // 이전의 픽스드 업데이트에서 버프를 못넣었다는 의미
                            if (buffElement.isStay == false)
                                break;
                            buffElement.UpdateTimer();
                        }

                        break;
                    }
                // 부착형
                case BuffType.Attachment:
                    {
                        ActiveBuff(buffElement);
                        while (true)
                        {
                            yield return WaitForFixedUpdate;
                            buffElement.UpdateTimer();
                        }

                    }
            }


            // 버프가 끝나면 버프 비활성화 및 제거
            RemoveBuff(casterID, buff);

            yield break;
        }

        public void ResetStack(BuffBase buff)
        {
            foreach (var item in m_buffList)
            {
                if (item.Value.Buff.ID == buff.ID)
                {
                    item.Value.ResetStack();
                }
            }

        }


        //TODO : 여기 버프 종류별로 다 바꿔야함.
        /// <summary>
        /// 한가지 버프 상태만 가져야하는 버프의 경우 버프 체크를 진행한다.
        /// 가장 높은 우선도를 체크하도록 한다.
        /// </summary>
        /// <param name="buff">체크할 버프</param>
        /// <param name="checkThis">체크할 버프를 함께 검사할지 정한다. 삭제할때는 필요가 없다.</param>
        public void OnlyOneBuffCheck(BuffBase buff, int checkThis = default)
        {
            // 기준을 정한다.
            BuffElement priorityBuff = null;

            foreach (var item in m_buffList)
            {

                // 같은 종류의 버프인지 체크한다.
                if (item.Value.Buff.ID != buff.ID)
                    continue;

                // 같은 버프 중 켜져있다면 일단 다 꺼준다.
                if (item.Value.isActive)
                {
                    item.Value.Buff.InactiveBuff(this);
                    item.Value.SetActive(false);
                }
                if (item.Key == checkThis)
                    continue;


                // 만약 비어있으면 넣는다.
                if (priorityBuff == null)
                {
                    priorityBuff = item.Value;
                    continue;
                }

                if (priorityBuff.Buff.Priority < item.Value.Buff.Priority)
                    priorityBuff = item.Value;
            }
            if (priorityBuff == null)
                return;

            // 선정된 버프를 활성화해준다.
            StopCoroutine(priorityBuff.Coroutine);
            priorityBuff.Coroutine = StartCoroutine(BuffRoutine(priorityBuff.ID, priorityBuff));

        }

        // 유지할 버프가 있는지 체크하고 유지한다.
        private bool MaintainBuffCheck(BuffElement buffElement, int checkThis = default)
        {
            BuffBase buff = buffElement.Buff;
            // 유일해야하는 버프가 아니라면 패스해도 된다.
            if (buff.IsOneAndOnly == false)
                return false;

            // 기준을 정한다.
            BuffElement priorityBuff = null;

            foreach (var item in m_buffList)
            {

                // 같은 종류의 버프인지 체크한다.
                if (item.Value.Buff.ID != buff.ID)
                    continue;

                // 같은 버프 중 켜져있다면 일단 다 꺼준다.
                if (item.Value.isActive)
                {
                    item.Value.Buff.InactiveBuff(this);
                    item.Value.SetActive(false);
                }
                if (item.Key == checkThis)
                    continue;


                // 만약 비어있으면 넣는다.
                if (priorityBuff == null)
                {
                    priorityBuff = item.Value;
                    continue;
                }

                if (priorityBuff.Buff.Priority < item.Value.Buff.Priority)
                    priorityBuff = item.Value;
            }
            if (priorityBuff == null)
                return false;

            // 계승한다.
            priorityBuff.MainTain(buffElement);


            // 선정된 버프를 활성화해준다.
            StopCoroutine(buffElement.Coroutine);
            StopCoroutine(priorityBuff.Coroutine);

            priorityBuff.Coroutine = StartCoroutine(BuffRoutine(priorityBuff.ID, priorityBuff));
            return true;
        }

        // 버프들은 상태 이상에 수정할 정보를 전달해준다.
        public void BuffStatus(Status newStatus)
        {
            m_status.Buff(newStatus);
        }

        // 버프들은 상태 이상에 수정할 정보를 전달해준다.
        public void DeBuffStatus(Status newStatus)
        {
            m_status.DeBuff(newStatus);
        }
        // 버프의 활성화를 체크한다.
        public bool BuffEnableCheck(GameObject caster, BuffBase buff)
        {
            int casterID = GFunc.XORCombine(caster.GetInstanceID(), buff.GetInstanceID());
            return m_buffList.ContainsKey(casterID);
        }

        private void StopCoroutine(BuffElement buffElement)
        {
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
            int[] ids = new int[m_buffList.Count];
            int count = 0;
            foreach (var id in m_buffList)
            {
                ids[count] = id.Key;
                count++;
            }

            // 버프 리스트 내의 모든 버프를 해제한다.
            for (int i = m_buffList.Count - 1; 0 <= i; i--)
            {
                RemoveBuff(ids[i], m_buffList[ids[i]].Buff);
            }
        }

    }
}