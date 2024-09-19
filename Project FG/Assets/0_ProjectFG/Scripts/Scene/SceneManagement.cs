using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JH
{
	public class SceneManagement : MonoBehaviour
	{
        [SerializeField] private string SceneName;
	 
        public void SceneChange()
        {
            SceneChange(SceneName);
        }


        public void SceneChange(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
	}
}
