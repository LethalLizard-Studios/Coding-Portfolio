using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/* All Rights Reserved to Leland T Carter of LethalLizard Studios.
 * @status COMPLETE
 * @last_change 2024-05-31
*/

namespace LLS.Crosshair
{
    public enum CrosshairComponent
    {
        FinNorth, FinEast, FinSouth, FinWest, CenterDot
    }

    //View of Crosshair Model-View-Presenter
    public class CrosshairView : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The parent to all crosshair components")]
        private GameObject parentHolder;

        //The parents of the fins activated in crosshair customization
        [SerializeField]
        private GameObject finNorthParent;
        [SerializeField]
        private GameObject finEastParent;
        [SerializeField]
        private GameObject finSouthParent;
        [SerializeField]
        private GameObject finWestParent;

        //The movable fins which spread out
        [SerializeField]
        private Transform[] fins;

        [SerializeField]
        private Transform centerDot;

        private float _currentSpread = RESTING_FIN_GAP;

        private const int RESTING_FIN_GAP = -12;

        private const float GROW_DURATION = 0.04f;
        private const float SHRINK_DURATION = 0.2f;

        //Enables or Disables the full crosshair and all components
        public void SetCrosshairActive(bool value)
        {
            parentHolder.SetActive(value);
        }

        //Enables or Disables only the fins of the crosshair
        public void SetCrosshairFinsActive(bool value)
        {
            for (int i = 0; i < fins.Length; i++)
                fins[i].gameObject.SetActive(value);
        }

        //Uses Tweening to animate the smooth and snappy spreading of the crosshair
        public void AnimateFinSpread(float strength)
        {
            float growTarget = RESTING_FIN_GAP - strength;

            DOTween.Kill(transform);

            //Spread out
            DOTween.To(() => _currentSpread, spread => SetFinSpread(spread), growTarget, GROW_DURATION)
                .SetTarget(transform)
                .OnComplete(() =>
                {  
                    //When done spreading out return back
                    DOTween.To(() => _currentSpread, spread => SetFinSpread(spread), RESTING_FIN_GAP, SHRINK_DURATION)
                    .SetTarget(transform);
                });
        }

        //Moves the fins X position to spread out evenly
        private void SetFinSpread(float spread)
        {
            _currentSpread = spread;

            Vector3 spreadPosition = new Vector3(_currentSpread, fins[0].localPosition.y, fins[0].localPosition.z);

            for (int i = 0; i < fins.Length; i++)
                fins[i].localPosition = spreadPosition;
        }

        #region Player Customization

        //Enables or Disables player specified components for customization
        public void SetCustomizationComponentActive(CrosshairComponent crosshairComponent, bool value)
        {
            switch (crosshairComponent)
            {
                case CrosshairComponent.FinNorth:
                    finNorthParent.SetActive(value);
                    break;
                case CrosshairComponent.FinEast:
                    finEastParent.SetActive(value);
                    break;
                case CrosshairComponent.FinSouth:
                    finSouthParent.SetActive(value);
                    break;
                case CrosshairComponent.FinWest:
                    finWestParent.SetActive(value);
                    break;
                case CrosshairComponent.CenterDot:
                    centerDot.gameObject.SetActive(value);
                    break;
            }
        }

        public void AdjustCenterDotScale(float scaleMultiplier)
        {
            centerDot.localScale *= scaleMultiplier;
        }

        public void AdjustColor(Color color)
        {
            for (int i = 0; i < fins.Length; i++)
                fins[i].GetComponent<Image>().color = color;
        }

        #endregion
    }
}
