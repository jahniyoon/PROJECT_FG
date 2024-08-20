using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;


namespace JH
{
    public static class GFunc
    {
        #region DEBUG
        // 디버그 로그를 찍어주는 메서드
        public static void Log(object _input)
        {
#if UNITY_EDITOR
            Debug.Log(_input);
#endif
        }

        // 디버그 로그를 찍어주는 메서드
        public static void LogWarning(string _input)
        {
#if UNITY_EDITOR

            Debug.LogWarning(_input);
#endif
        }

        // 디버그 로그를 찍어주는 메서드
        public static void LogError(string _input)
        {
#if UNITY_EDITOR
            Debug.LogError(_input);
#endif
        }
        #endregion

        private static StringBuilder stringBuilder = new StringBuilder();

        /// <summary>
        /// 매개 변수로 받은 모든 string 인자를 더해서 반환한다.
        /// </summary>
        public static string SumString(params string[] inputs)
        {
            stringBuilder.Clear();
            for (int i = 0; i < inputs.Length; i++)
            {
                stringBuilder.Append(inputs[i]);
            }

            return stringBuilder.ToString();
        }
        public static string[] Split(string input)
        {
            string[] words = input.Split(',');
            return words;
        }



        // 각도를 제한하는 함수
        public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }


        // 데시벨을 선형으로 변환해주는 함수
        public static float DBToLinear(float dB)
        {
            float linear = Mathf.Log10(dB) * 20;
            return linear;
        }

        public static int GetFirstLayerInMask(LayerMask mask)
        {
            int maskValue = mask.value;
            for (int i = 0; i < 32; i++)
            {
                if ((maskValue & (1 << i)) != 0)
                {
                    return i; // 첫 번째 활성화된 레이어 인덱스를 반환
                }
            }
            return -1; // 활성화된 레이어가 없을 경우 -1 반환
        }



        //Breadth-first search
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            // 큐에 넣기
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);

            // 모든 큐 돌기
            while (0 < queue.Count)
            {
                // 부모 캐싱하고
                var c = queue.Dequeue();

                // 이름이 같은지 체크
                if (c.name == aName)
                    return c;

                // 자식 모두 큐에 넣기
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }

        // 네비게이션 위치를 찾는 함수
        public static Vector3 FindNavPos(Transform agent, Vector3 targetPoint, float range)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPoint, out hit, range, NavMesh.AllAreas))
            {
                Vector3 hitPos = hit.position;
                Vector3 position = agent.position;
                hitPos.y += 0.5f;
                position.y = hitPos.y;

                //RaycastHit rayHit;
                //int layerMask = 1 << LayerMask.NameToLayer("Wall");
                //if (Physics.Raycast(hitPos, (hitPos - position).normalized * -1, out rayHit, range, layerMask))
                //{
                //    return agent.position;
                //}

                return hit.position;
            }
            Debug.Log(agent.gameObject.name + " Nav Destination Missing");
            return agent.position;
        }
    }
}