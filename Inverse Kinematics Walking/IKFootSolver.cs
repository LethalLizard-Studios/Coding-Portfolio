using UnityEngine;

/* All Rights Reserved to Leland T Carter of LethalLizard Studios.
 * @status COMPLETE
 * @date 2021-07-15
*/

namespace LLS.IK
{
    public class IKFootSolver : MonoBehaviour
    {
        [SerializeField]
        private LayerMask walkableLayer;

        [SerializeField]
        private Transform body;

        [SerializeField]
        private IKFootSolver oppositeFoot;

        [SerializeField]
        private float speed = 5;

        [SerializeField]
        private Vector3 footPositionOffset, footEulerRotationOffset;

        private float _footSpacing, _lerpSpeed;

        private Vector3 _oldPosition, _currentPosition, _newPosition;
        private Vector3 _oldNormal, _currentNormal, _newNormal;

        private bool _isFirstStep = true;

        private const float STEP_DISTANCE = 0.3f;
        private const float STEP_LENGTH = 0.3f;
        private const float STEP_HEIGHT = 0.3f;

        private const int MAX_RAYCAST_DISTANCE = 10;

        private void Start()
        {
            //Initialize private values
            _footSpacing = transform.localPosition.x;
            _currentPosition = _newPosition = _oldPosition = transform.position;
            _currentNormal = _newNormal = _oldNormal = transform.up;

            _lerpSpeed = 1;
        }

        public bool IsMoving() { return _lerpSpeed < 1; }

        private void Update()
        {
            transform.position = _currentPosition + footPositionOffset;
            transform.rotation = Quaternion.LookRotation(_currentNormal) * Quaternion.Euler(footEulerRotationOffset);

            Ray ray = new(body.position + (body.right * _footSpacing) + (Vector3.up * 2), Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, MAX_RAYCAST_DISTANCE, walkableLayer.value))
            {
                if (_isFirstStep || (LLS.Math.Distance(_newPosition, hit.point) > STEP_DISTANCE && !oppositeFoot.IsMoving() && !IsMoving()))
                {
                    _lerpSpeed = 0;

                    int dir = body.InverseTransformPoint(
                        hit.point).z > body.InverseTransformPoint(_newPosition).z ? 1 : -1;

                    //Set the steps position based of the bodies rotation, direction, and steps set length
                    _newPosition = hit.point + (body.forward * (dir * STEP_LENGTH));
                    _newNormal = hit.normal;

                    _isFirstStep = false;
                }
            }

            if (_lerpSpeed < 1)
            {
                //Step towards the bodies forward
                Vector3 targetPosition = Vector3.Lerp(_oldPosition, _newPosition, _lerpSpeed);
                targetPosition.y += Mathf.Sin(_lerpSpeed * Mathf.PI) * STEP_HEIGHT;

                _currentPosition = targetPosition;
                _currentNormal = Vector3.Lerp(_oldNormal, _newNormal, _lerpSpeed);

                _lerpSpeed += Time.deltaTime * speed;
            }
            else
            {
                //Set most recent steps position and normal to old
                _oldPosition = _newPosition;
                _oldNormal = _newNormal;
            }
        }

        //Debug visuals
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_newPosition, 0.1f);
        }
    }
}
