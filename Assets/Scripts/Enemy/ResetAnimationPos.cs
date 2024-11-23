using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimationPos : MonoBehaviour
{
    private Animator _animator;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    void Start()
    {
        // Salva a posição e rotação originais
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        
        _animator = GetComponent<Animator>();
    }

    public void PlayAnimationAndReset(string animationName)
    {
        // Toca a animação
        _animator.Play(animationName);

        // Reset a posição e rotação após a animação
        StartCoroutine(ResetPositionAfterAnimation());
    }

    private IEnumerator ResetPositionAfterAnimation()
    {
        // Aguarda até que a animação termine
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        // Volta para a posição e rotação iniciais
        transform.position = _originalPosition;
        transform.rotation = _originalRotation;
    }
}
