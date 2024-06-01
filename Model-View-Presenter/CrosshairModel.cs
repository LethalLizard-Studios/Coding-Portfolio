using UnityEngine;

/* All Rights Reserved to Leland T Carter of LethalLizard Studios.
 * @status COMPLETE
 * @last_change 2024-05-31
*/

namespace LLS.Crosshair
{
    //Model of Crosshair Model-View-Presenter
    public class CrosshairModel : MonoBehaviour
    {
        public delegate void AddRecoil(float stength);
        public AddRecoil AddedRecoilCallback;

        private const int RECOIL_MULTIPLIER = 24;

        //Used to add a callback to recoil changes
        public void ListenToRecoilAdded(AddRecoil addedRecoil)
        {
            AddedRecoilCallback = addedRecoil;
        }

        //Used to set the crosshairs current recoil (Ex. When shooting)
        public void SetRecoil(float stength)
        {
            AddedRecoilCallback(stength * RECOIL_MULTIPLIER);
        }
    }
}
