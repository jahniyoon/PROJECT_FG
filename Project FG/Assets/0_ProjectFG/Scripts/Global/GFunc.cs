using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
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

        private static StringBuilder sb = new StringBuilder();

        /// <summary>
        /// 매개 변수로 받은 모든 string 인자를 더해서 반환한다.
        /// </summary>
        public static string SumString(params string[] inputs)
        {
            sb.Clear();
            for (int i = 0; i < inputs.Length; i++)
            {
                sb.Append(inputs[i]);
            }

            return sb.ToString();
        }
        public static string[] Split(string input)
        {
            string[] words = input.Split(',');
            return words;
        }

        public static float[] StringToFloats(string input)
        {
            string[] str = input.Split(", ");

            return str.Select(float.Parse).ToArray(); 

        }
        public static int[] StringToInts(string input)
        {
            string[] str = input.Split(", ");

            return str.Select(int.Parse).ToArray();

        }
        public static string IntsToString(int[] id)
        {
            sb.Clear();
            if (id == null || id.Length == 0)
                return "-";

            for (int i = 0; i < id.Length; i++)
            {
                sb.Append(id[i]);
                if(i != id.Length - 1)
                    sb.Append(", ");
            }
            return sb.ToString();
        }
        public static string FloatsToString(float[] id)
        {
            sb.Clear();
            if (id == null || id.Length == 0)
                return "-";


            for (int i = 0; i < id.Length; i++)
            {
                sb.Append(id[i]);
                if (i != id.Length - 1)
                    sb.Append(", ");
            }
            return sb.ToString();
        }

        public static List<BuffValues> StringToBuffValues(string input)
        {
            List<BuffValues> buffValues = new List<BuffValues>();
            buffValues.Clear();

            string[] values = input.Replace("]", "[").Split("[");

            for (int i = 0; i < values.Length; i++)
            {
                // 비어있으면 패스
                if (values[i].NullIfEmpty() == null) continue;

                float[] floats = GFunc.StringToFloats(values[i]);
                buffValues.Add(new BuffValues(floats));
            }

            return buffValues;
        }
        public static string BuffValuesToString(List<BuffValues> values)
        {
            sb.Clear();
            if(values == null || values.Count == 0) return "-";
            
            for(int i = 0; i < values.Count; i++)
            {
                sb.Append("[");
                for(int j = 0; j < values[i].Length; j++)
                {
                    string floats = GFunc.FloatsToString(values[i].Values);
                    sb.Append(floats);
                }
                sb.Append("]");
            }
            return sb.ToString();
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

        // 타겟의 방향 각도를 체크하는 함수
        public static bool TargetAngleCheck(Transform Start, Transform Target, float Angle)
        {
            // 정확한 방향을 체크하기 위해 높이를 우선 맞춰준다.
            Vector3 TargetPosition = Target.position;
            TargetPosition.y = Start.position.y;

            Vector3 Direction = TargetPosition - Start.position;
            // 방향 0 기준으로, -180 ~ 180까지로 변환
            float SignedAngle = Vector3.SignedAngle(Start.forward, Direction, Vector3.up);
            // 음수 양수를 절대값으로 체크하므로 반 나눈다.
            return Mathf.Abs(SignedAngle) <= Angle / 2;

        }

        public static List<GSTU_Data> GetGameData(int ID)
        {
            DataReader gameData = Resources.Load<DataReader>("Data/GameData");
            if (gameData.GameData.ContainsKey(ID) == false)
            {
                Debug.LogWarning("데이터 ID를 확인해주세요." + ID);
                return null;
            }
            return gameData.GameData[ID].Data;
        }

        public static SkillBase GetSkillPrefab(int skillID)
        {
            PrefabData prefabData = Resources.Load<PrefabData>("Data/PrefabData");           
            return prefabData.TryGetSkill(skillID);
        }
        public static ProjectileBase GetProjectilePrefab(int projectileID)
        {
            PrefabData prefabData = Resources.Load<PrefabData>("Data/PrefabData");
            return prefabData.TryGetProjectile(projectileID);
        }
        public static FoodPower GetFoodPowerPrefab(int foodPowerID)
        {
            PrefabData prefabData = Resources.Load<PrefabData>("Data/PrefabData");
            return prefabData.TryGetFoodPower(foodPowerID);
        }
        public static BuffBase TryGetBuff(int buffID)
        {
            BuffData data = GetBuffData(buffID);
            if(data == null)
                return null;

            return BuffFactory.CreateBuff(data);

        }
        public static BuffData GetBuffData(int buffID)
        {
            BuffDataBase buffData = Resources.Load<BuffDataBase>("Data/BuffData");
            return buffData.TryGetBuff(buffID);
        }
        public static BuffDataBase BuffData()
        {
            return Resources.Load<BuffDataBase>("Data/BuffData");
        }


        public static int XORCombine(int x, int y)
        {
            return x ^ y;  // 비트 단위 XOR 연산
        }

    }
}