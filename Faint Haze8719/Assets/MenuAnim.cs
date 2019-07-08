using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuAnim : MonoBehaviour
{

    public Sprite first;
    public Sprite second;
    public Sprite third;
    public Sprite forth;
    private Image actualSprite;
    // Start is called before the first frame update
    void Start()
    {
        actualSprite = GetComponent<Image>();

        StartCoroutine("SpriteBehaviour");
    }

   IEnumerator SpriteBehaviour()
    {
        while (true)
        {
            actualSprite.sprite = second;
            yield return new WaitForSeconds(1.0f);

            actualSprite.sprite = third;
            yield return new WaitForSeconds(1.0f);

            actualSprite.sprite = forth;
            yield return new WaitForSeconds(1.0f);

            actualSprite.sprite = first;
            yield return new WaitForSeconds(1.0f);
        }
    }
}
