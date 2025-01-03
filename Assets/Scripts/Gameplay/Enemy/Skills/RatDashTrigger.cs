using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RatDashTrigger : MonoBehaviour
{ 
    [SerializeField] private Rigidbody2D _rigidBody;
    private Rat _rat;

    private float _dashTimer = 0f;
    private int _dashDelay = 30;

    public void Init(Rat rat)
    {
        _rat = rat;
    }

    private void Update()
    {
        if (_rigidBody.IsSleeping())
            _rigidBody.WakeUp();

        _dashTimer += Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {     
        if (_dashTimer < _dashDelay)
            return;

        if (collision.gameObject.TryGetComponent(out Head head))
        {
            if(head.transform == _rat.Target)
            {
                _rat.Animator.SetTrigger(Rat.AnimationController.Dash);
                _dashTimer = 0f;
            }
        }
    }
}
