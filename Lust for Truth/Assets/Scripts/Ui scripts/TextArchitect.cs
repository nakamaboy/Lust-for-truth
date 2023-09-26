using System.Collections;
using UnityEngine;
using TMPro;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using Unity.VisualScripting.Dependencies.Sqlite;

public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_world;
    
    // code that makes tmpro be able to accept both ui tmpro and world tmpro.
    // != is short for "not equal to" and ? is an if statement and if it's true do X which is written before :
    // If false, do Y which is written after :     
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    public string currentText => tmpro.text;

    public string targetText { get; private set; } = "";

    public string preText { get; private set; } = "";

    private int preTextLength = 0;

    public string fullTargetText => preText + targetText;

    public enum BuildMethod { instant, typewriter, fade }
    public BuildMethod buildMethod = BuildMethod.typewriter;

    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }
    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private const float baseSpeed = 2;
    private float speedMultiplier = 2;

    public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : 
                speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3;} }
    private int characterMultiplier = 1;

    public bool hurryUP;

    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }
    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }
    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }
    // Append text to what already has been built in the text architect 
    public Coroutine Append(string text)
    {
        preText = tmpro.text;
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }
    private Coroutine buildProcess = null;

    public bool isBuilding => buildProcess != null;

    public void Stop()
    {
        if (!isBuilding)
            return;
        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    IEnumerator Building()
    {
        Prepare();
        switch (buildMethod)
        {
            case BuildMethod.instant:
                yield return Build_Instant();
                break;
            case BuildMethod.typewriter:
                yield return Build_Typewriter();
                break;
            case BuildMethod.fade:
                yield return Build_Fade();
                break;
        }
    }
    private void OnComplete()
    {
        buildProcess = null;
        hurryUP = false;
    }
    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
            case BuildMethod.fade:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
        }
        Stop();
        OnComplete();
    }
    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.instant:
                Prepare_Instant();
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.fade:
                Prepare_Fade();
                break;

        }
        
    }
    private void Prepare_Instant()
    {
        tmpro.color = tmpro.color;
        tmpro.text = fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        
    }

    private void Prepare_Typewriter()
    {
       
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;
        //if (preText != "")
        //{
        //    tmpro.ForceMeshUpdate();
        //    tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;

        //}
        tmpro.text += targetText;
        //tmpro.ForceMeshUpdate(); 
        

    }
    private void Prepare_Fade()
    {
        tmpro.text = preText;
        if (preText != "")
        {
            tmpro.ForceMeshUpdate();
            preTextLength = tmpro.textInfo.characterCount;
        }
        else
            preTextLength = 0;
        tmpro.text += targetText;
        tmpro.maxVisibleCharacters = int.MaxValue;
        tmpro.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpro.textInfo;

        Color colorVisible = new Color(textColor.r, textColor.g, textColor.b, 1);
        Color colorHidden = new Color(textColor.r, textColor.g, textColor.b, 0);

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;
            if (i < preTextLength)
            {
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorVisible;
            }
            else
            {
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorHidden;
            }
        }
        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
    private IEnumerator Build_Instant()
    {
        yield return null;

    }
    private IEnumerator Build_Typewriter()
    {
        while(tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            //tmpro.maxVisibleCharacters += hurryUP ? charactersPerCycle * 5 : charactersPerCycle;
            tmpro.maxVisibleCharacters += charactersPerCycle;
            yield return new WaitForSeconds(0.015f / speed);
            
        }
        if (tmpro.maxVisibleCharacters >= tmpro.textInfo.characterCount)
        {
            buildProcess = null;
        }
    }
    private IEnumerator Build_Fade()
    {
        int minRange = preTextLength;
        int maxRange = minRange + 1;
        byte alphaThreshold = 15;

        TMP_TextInfo textInfo = tmpro.textInfo;

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];
            
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            float fadespeed = charactersPerCycle * 2f;

            for (int i = minRange; i < maxRange; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                
                alphas[i] = Mathf.MoveTowards(alphas[i], 255, fadespeed);

                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v].a = (byte)alphas[i];

                if (alphas[i] >= 255)
                {
                    minRange++;

                }
                    
            }

            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            bool lastCharacterIsInvisible = !textInfo.characterInfo[maxRange - 1].isVisible;
            if (alphas[maxRange - 1] > alphaThreshold || lastCharacterIsInvisible)
            {
                if (maxRange < textInfo.characterCount)
                    maxRange++;
                else if (alphas[maxRange - 1] >= 255 || lastCharacterIsInvisible)
                    break;
            }
            
            yield return new WaitForEndOfFrame();
            
        }
        if (tmpro.maxVisibleCharacters >= tmpro.textInfo.characterCount)
        {
            buildProcess = null;
        }

    }
}
