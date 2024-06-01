using UnityEngine;

/* All Rights Reserved to Leland T Carter of LethalLizard Studios.
 * @status COMPLETE
 * @last_change 2024-05-31
*/

namespace LLS.Crosshair
{
    //Presenter of Crosshair Model-View-Presenter
    public class CrosshairPresenter : MonoBehaviour
    {
        public CrosshairView view;
        public CrosshairModel model;

        private void Start()
        {
            //Create a callback to listen to any changes in recoil
            model.ListenToRecoilAdded(RecoilCallback);
        }

        //Callback for when recoil is added to the model
        public void RecoilCallback(float stength)
        {
            view.AnimateFinSpread(stength);
        }
    }
}
