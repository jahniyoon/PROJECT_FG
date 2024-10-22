using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
	public interface IPutrefaction
	{
        
        /// <summary>
        /// 부패 리스트에 해당 버프를 추가
        /// </summary>
        /// <param name="putrefaction"></param>
        public void AddPuterefaction(Putrefaction putrefaction);
        /// <summary>
        /// 부패 리스트에서 해당 부패를 제거
        /// </summary>
        /// <param name="putrefaction"></param>
        public void RemovePuterefaction(Putrefaction putrefaction);

        public void SetPutrefactionOver(Putrefaction putrefaction);
	}
}
