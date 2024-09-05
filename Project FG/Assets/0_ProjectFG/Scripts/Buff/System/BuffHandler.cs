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
        [SerializeField] private SerializableDictionary<int, BuffBase> m_buffList = new SerializableDictionary<int, BuffBase>();

        [Header("기본 버프")]
        [SerializeField] private BuffBase[] m_inherenceBuff;

        #region Property
        public Status Status => m_status;
        #endregion

        [Header("Buff Event")]
        [HideInInspector] public UnityEvent AddBuffEvent;
        [HideInInspector] public UnityEvent RemoveBuffEvent;

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
            int casterID = caster.GetInstanceID();

            // 버프가 이미 있으면 스택을 쌓아준다.
            if (m_buffList.ContainsKey(casterID))
            {
                buff.StackBuff(this);
                return;
            }

            // 버프를 추가한다.
            AddBuff(casterID, buff);
        }

        // 버프를 추가한다.
        private void AddBuff(int casterID, BuffBase buff)
        {
            // 리스트에 버프 추가하고
            m_buffList.Add(casterID, buff);
            // 버프 코루틴을 돌린다.
            StartCoroutine(BuffRoutine(casterID, buff));

            AddBuffEvent?.Invoke();
        }

        // 버프를 제거한다.
        private void RemoveBuff(int casterID, BuffBase buff)
        {
            // 버프가 있다면 버프 비활성화 및 리스트에서 제거
            if (m_buffList.ContainsKey(casterID))
            {
                buff.InactiveBuff(this);
                m_buffList.Remove(casterID);

                // 제거를 하고나면 버프를 체크한다.
                BuffCheck(buff);
            }

            RemoveBuffEvent?.Invoke();
        }

        // 받아온 버프를 실행한다.
        IEnumerator BuffRoutine(int casterID, BuffBase buff)
        {
            float timer = 0;
            float activeTimer = 0;  // 조건을 검사해야하는 듀레이션이 필요한 경우

            // 버프를 추가하고, 버프 체크를 진행한다.
            buff.ActiveBuff(this);
            BuffCheck(buff);

            // 지속시간동안 유지한다.
            while (timer < buff.Duration)
            {
                // 조건식이 맞으면 조건식 버프가 실행된다.
                if (buff.CanActive(timer))
                {
                    buff.ConditionBuff(this);
                    activeTimer = 0;    // 액티브 타이머는 리셋해준다.
                }

                // 타이머 업데이트
                timer += Time.deltaTime;
                activeTimer += Time.deltaTime;
                yield return null;
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
        public void BuffCheck(BuffBase buff, bool checkThis = true)
        {
            // 유일해야하는 버프가 아니라면 패스해도 된다.
            if (buff.IsOneAndOnly == false)
                return;

            // 기준을 정한다.
            BuffBase priorityBuff = checkThis ? buff : null;

            foreach (var item in m_buffList)
            {
                // 같은 버프인지 체크한다.
                if (item.Value.ID != buff.ID)
                    continue;

                // 같은 버프라면 일단 다 꺼준다.
                item.Value.InactiveBuff(this);

                // 만약 비어있으면 넣는다.
                if (priorityBuff == null)
                {
                    priorityBuff = item.Value;
                    continue;
                }

                if (priorityBuff.Priority < item.Value.Priority)
                    priorityBuff = item.Value;
            }

            // 선정된 버프를 활성화해준다.
            priorityBuff.ActiveBuff(this);
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
            int casterID = caster.GetInstanceID();
            return m_buffList.ContainsKey(casterID) && m_buffList.ContainsValue(buff);
        }
        public void RemoveAllBuff()
        {
            // 버프 리스트 내의 모든 버프를 해제한다.
            foreach (var buff in m_buffList)
                RemoveBuff(buff.Key, buff.Value);
        }

    }
}