using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        public bool open; // Indica se a porta está aberta
        public float smooth = 1.0f;
        private float DoorOpenAngle = -90.0f; // Ângulo para abrir a porta
        private float DoorCloseAngle = 0.0f;  // Ângulo para fechar a porta
        private AudioSource asource;
        public AudioClip openDoor, closeDoor;

        void Start()
        {
            asource = GetComponent<AudioSource>();
        }

        void Update()
        {
            // Controle da rotação da porta
            if (open)
            {
                var target = Quaternion.Euler(0, DoorOpenAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
            }
            else
            {
                var target1 = Quaternion.Euler(0, DoorCloseAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target1, Time.deltaTime * 5 * smooth);
            }
        }

        // Método para abrir a porta
        public void Open()
        {
            if (!open) // Apenas abre se já não estiver aberta
            {
                open = true;
                PlaySound(openDoor);
            }
        }

        // Método para fechar a porta
        public void Close()
        {
            if (open) // Apenas fecha se já não estiver fechada
            {
                open = false;
                PlaySound(closeDoor);
            }
        }

        // Reproduz os sons de abertura e fechamento
        private void PlaySound(AudioClip clip)
        {
            if (asource != null && clip != null)
            {
                asource.clip = clip;
                asource.Play();
            }
        }
    }
}
