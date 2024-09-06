using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JH
{
    [System.Serializable]
    public class BuffElement
    {
        public int Stack = 1;
        public float Timer = 0;
        public bool isActive = false;
        public BuffBase Buff;
        public void StackUp()
        {
            Timer = 0;
            Stack++;
        }
        public void StackDown()
        {
            Stack--;
        }

    }
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

        private void Awake()
        {
        }

        private void Start()
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
            m_status.Init();

            int casterID = this.gameObject.GetInstanceID();

            // 장착된 버프들을 활성화한다.
            foreach (var buff in m_inherenceBuff)
                AddBuff(casterID, buff);
        }

        // 버프를 받는 부분 (파스 붙이는 부분)
        public void OnBuff(GameObject caster, BuffBase buff)
        {
            // 키가 중복되지 않도록 하기
            int casterID = caster.GetInstanceID() + buff.GetInstanceID();

            // 버프가 이미 있으면 스택을 쌓아준다.
            if (m_buffList.ContainsKey(casterID))
            {
                if (buff.StackUpTime < m_buffList[casterID].Timer)
                {
                    m_buffList[casterID].Buff.StackBuff(this);
                    m_buffList[casterID].StackUp();
                }
                m_buffList[casterID].Timer += (Time.deltaTime * 2);
                return;
            }

            // 버프를 추가한다.
            AddBuff(casterID, buff);
        }

        // 버프를 추가한다.
        private void AddBuff(int casterID, BuffBase buff)
        {
            BuffElement newBuff = new BuffElement();
            newBuff.Buff = buff;
            newBuff.Timer = buff.DecreaseTime;

            // 리스트에 버프 추가하고
            m_buffList.Add(casterID, newBuff);

            // 버프 코루틴을 돌린다.
            StartCoroutine(BuffRoutine(casterID, newBuff));

            AddBuffEvent?.Invoke();
        }

        // 버프를 제거한다.
        public void RemoveBuff(int casterID, BuffBase buff)
        {
            // 버프가 있다면 버프 비활성화 및 리스트에서 제거
            if (m_buffList.ContainsKey(casterID))
            {
                if (m_buffList[casterID].isActive)
                    buff.InactiveBuff(this);

                m_buffList.Remove(casterID);

                // 제거를 하고나면 버프를 체크한다.
                BuffCheck(buff, casterID);
            }

            RemoveBuffEvent?.Invoke();
        }
        // 버프를 제거한다.
        public void RemoveBuff(GameObject caster, BuffBase buff)
        {
            int casterID = caster.GetInstanceID() + buff.GetInstanceID();
            // 버프가 있다면 버프 비활성화 및 리스트에서 제거
            if (m_buffList.ContainsKey(casterID))
            {
                buff.InactiveBuff(this);
                m_buffList.Remove(casterID);

                // 제거를 하고나면 버프를 체크한다.
                BuffCheck(buff, casterID);
            }

            RemoveBuffEvent?.Invoke();
        }

        public void ActiveBuff(BuffElement buffElement)
        {
            // 이미 켜져있으면 또 킬필요 없음
            if (buffElement.isActive)
                return;


            buffElement.Buff.ActiveBuff(this);
            buffElement.isActive = true;

            BuffCheck(buffElement.Buff);
        }

        // 받아온 버프를 실행한다.
        IEnumerator BuffRoutine(int casterID, BuffElement buffElement)
        {
            BuffBase buff = buffElement.Buff;


            // 타입에 따라 루틴이 달라진다.
            switch (buff.Type)
            {
                // 1. 즉시 효과형
                case BuffType.Immediately:
                    {
                        float timer = 0;

                        // 즉시 실행하고 지속시간을 기다린다.
                        ActiveBuff(buffElement);
                        while (timer < buff.Duration)
                        {
                            timer += Time.deltaTime;
                            yield return null;
                        }
                        break;
                    }
                //2. 조건 확인형 : 지속시간동안 조건을 충족하는지 기다린다.
                case BuffType.Condition:
                    {
                        float timer = 0;
                        buffElement.Timer += 0.1f;
                        // 지속시간동안 유지한다.
                        while (0 < buffElement.Timer)
                        {
                            // 조건식이 맞으면 조건식 버프가 실행된다.
                            if (buff.CanActive(timer))
                            {
                                ActiveBuff(buffElement);
                                buffElement.isActive = false;
                                timer = 0;    // 액티브 타이머는 리셋해준다.
                            }

                            // 타이머 업데이트
                            timer += Time.deltaTime;

                            // 타이머를 줄여준다.
                            buffElement.Timer -= Time.deltaTime;
                            yield return null;
                        }
                        break;
                    }

                case BuffType.Stack:
                    {
                        // 스택이 쌓이는 방식일 경우
                        while (0 <= buffElement.Stack)
                        {
                            if (buffElement.Timer < 0)
                            {
                                buffElement.StackDown();
                                buffElement.Timer = buff.DecreaseTime;
                            }

                            if (buff.CanActive(buffElement.Stack))
                            {
                                ActiveBuff(buffElement);
                                buffElement.Stack = 0;
                                buffElement.Timer = buff.DecreaseTime;
                            }
                            buffElement.Timer -= Time.deltaTime;
                            yield return null;
                        }
                        break;
                    }
            }
            // 버프가 끝나면 버프 비활성화 및 제거
            RemoveBuff(casterID, buff);

            yield break;
        }
        /// <summary>
        /// 한가지 버프 상태만 가져야하는 버프의 경우 버프 체크를 진행한다.
        /// 가장 높은 우선도를 체크하도록 한다.
        /// </summary>
        /// <param name="buff">체크할 버프</param>
        /// <param name="checkThis">체크할 버프를 함께 검사할지 정한다. 삭제할때는 필요가 없다.</param>
        public void BuffCheck(BuffBase buff, int checkThis = default)
        {
            // 유일해야하는 버프가 아니라면 패스해도 된다.
            if (buff.IsOneAndOnly == false)
                return;

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
                    item.Value.isActive = false;
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
            priorityBuff.Buff.ActiveBuff(this);
            priorityBuff.isActive = true;
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
            int casterID = caster.GetInstanceID() + buff.GetInstanceID();
            return m_buffList.ContainsKey(casterID);
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