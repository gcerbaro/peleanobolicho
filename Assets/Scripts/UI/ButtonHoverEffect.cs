using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image buttonImage;
    private TextMeshProUGUI buttonText;

    private Color originalImageColor;
    private Color originalTextColor;

    [SerializeField] private Color hoverImageColor = Color.white; // Cor da imagem ao passar o mouse
    [SerializeField] private Color hoverTextColor = Color.black; // Cor do texto ao passar o mouse
    [SerializeField] private float fadeDuration = 0.1f; // Duração do fade

    private Coroutine imageFadeCoroutine;
    private Coroutine textFadeCoroutine;

    private void Awake()
    {
        // Obtém o componente Image do botão
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        
        if (!buttonImage) Debug.LogError("Componente Image não encontrado no botão!");
        if (!buttonText)  Debug.LogError("Componente Text não encontrado no botão!");
        
        originalImageColor = buttonImage.color; // Salva a cor original
        originalTextColor = buttonText.color; // Salva a cor original do texto
    }

    // Chamado quando o mouse entra no botão
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartImageFade(hoverImageColor); // Inicia o fade para a cor de hover da imagem
        
        StartTextFade(hoverTextColor); // Inicia o fade para a cor de hover do texto
    }

    // Chamado quando o mouse sai do botão
    public void OnPointerExit(PointerEventData eventData)
    {
        StartImageFade(originalImageColor); // Restaura a cor original com fade
        StartTextFade(originalTextColor); // Restaura a cor original do texto com fade
    }

    private void StartImageFade(Color targetColor)
    {
        if (imageFadeCoroutine != null) StopCoroutine(imageFadeCoroutine);
        
        imageFadeCoroutine = StartCoroutine(FadeToColor(buttonImage, targetColor));
    }

    private void StartTextFade(Color targetColor)
    {
        if (textFadeCoroutine != null) StopCoroutine(textFadeCoroutine);
        
        textFadeCoroutine = StartCoroutine(FadeToColor(buttonText, targetColor));
    }

    private IEnumerator FadeToColor(Graphic graphic, Color targetColor)
    {
        Color currentColor = graphic.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // Interpola a cor gradualmente
            graphic.color = Color.Lerp(currentColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Espera o próximo frame
        }

        // Garante que a cor final seja exatamente a desejada
        graphic.color = targetColor;
    }
}
