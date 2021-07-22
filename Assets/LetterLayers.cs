using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using rnd = UnityEngine.Random;
public class LetterLayers : MonoBehaviour
{
    public KMSelectable[] upArrows;
    public KMSelectable[] downArrows;
    public KMSelectable deliver;
    public KMSelectable submit;
    public TextMesh[] displayTexts;
    public TextMesh[] questionTexts;
    public TextMesh answerText;
    public MeshRenderer[] LEDs;
    public MeshRenderer submitHL;
    public Material[] usefulMats;
    string[] languages = new string[] { "E", "Gr", "K", "R", "T", "JK", "JH" };
    string allCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZαβγδεζηθιλµξπρστφψωㄱㄲㄷㄹㅁㅂㅅㅆㅈㅊㅋㅌㅍㅎㅏㅐㅑㅒㅔㅖㅗㅘㅙㅚㅛㅝㅞㅟㅡㅢабгдеёжзийклпруфцчшщъыьэюяกขฃคฅงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรลวศษสหฬฃฮァイウエオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモヤユヨラリルレロワヰヱヲンヴヷヸヹヺあいうえおかがきぎくぐけげこごさざしじすずせぜそぞただちぢつづてでとどなにぬねのはばぱひびぴふぶぷほぼぽまみむめもやゆよらりるれろわゐゑをんゔ";
    string[][] languageArrays = new string[][]{
    new string[]{"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"},
    new string[]{"α","β","γ","δ","ε","ζ","η","θ","ι","λ","µ","ξ","π","ρ","σ","τ","φ","ψ","ω"},
    new string[]{"ㄱ","ㄲ","ㄷ","ㄹ","ㅁ","ㅂ","ㅅ","ㅆ","ㅈ","ㅊ","ㅋ","ㅌ","ㅍ","ㅎ","ㅏ","ㅐ","ㅑ","ㅒ","ㅔ","ㅖ","ㅗ","ㅘ","ㅙ","ㅚ","ㅛ","ㅝ","ㅞ","ㅟ","ㅡ","ㅢ"},
    new string[]{"а","б","г","д","е","ё","ж","з","и","й","к","л","п","р","у","ф","ц","ч","ш","щ","ъ","ы","ь","э","ю","я"},
    new string[]{"ก","ข","ฃ","ค","ฅ","ง","จ","ฉ","ช","ซ","ฌ","ญ","ฎ","ฏ","ฐ","ฑ","ฒ","ณ","ด","ต","ถ","ท","ธ","น","บ","ป","ผ","ฝ","พ","ฟ","ภ","ม","ย","ร","ล","ว","ศ","ษ","ส","ห","ฬ","ฃ","ฮ"},
    new string[]{"ァ","イ","ウ","エ","オ","カ","ガ","キ","ギ","ク","グ","ケ","ゲ","コ","ゴ","サ","ザ","シ","ジ","ス","ズ","セ","ゼ","ソ","ゾ","タ","ダ","チ","ヂ","ツ","ヅ","テ","デ","ト","ド","ナ","ニ","ヌ","ネ","ノ","ハ","バ","パ","ヒ","ビ","ピ","フ","ブ","プ","ヘ","ベ","ペ","ホ","ボ","ポ","マ","ミ","ム","メ","モ","ヤ","ユ","ヨ","ラ","リ","ル","レ","ロ","ワ","ヰ","ヱ","ヲ","ン","ヴ","ヷ","ヸ","ヹ","ヺ"},
    new string[]{"あ","い","う","え","お","か","が","き","ぎ","く","ぐ","け","げ","こ","ご","さ","ざ","し","じ","す","ず","せ","ぜ","そ","ぞ","た","だ","ち","ぢ","つ","づ","て","で","と","ど","な","に","ぬ","ね","の","は","ば","ぱ","ひ","び","ぴ","ふ","ぶ","ぷ","ほ","ぼ","ぽ","ま","み","む","め","も","や","ゆ","よ","ら","り","る","れ","ろ","わ","ゐ","ゑ","を","ん","ゔ"}};
    string[] question = new string[3];
    List<float> rotationChoices = new List<float> { 0.4f, 0.7f, 1f };
    int[] rotationDirs = new int[3];
    int[] displayIndices = new int[2];
    int stage;
    bool striking;
    bool activated;
    public KMBombModule module;
    public KMAudio sound;
    int moduleId;
    static int moduleIdCounter = 1;
    bool solved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        for (int i = 0; i < 2; i++)
        {
            int j = i;
            upArrows[i].OnInteract += delegate { PressArrow(true, j); return false; };
            downArrows[i].OnInteract += delegate { PressArrow(false, j); return false; };
        }
        for (int i = 0; i < 3; i++)
            questionTexts[i].text = "";
        deliver.OnInteract += delegate { deliver.AddInteractionPunch(); sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, deliver.transform); if (!solved && !striking && activated && answerText.text.Length < 3) answerText.text += (displayTexts[1].text); return false; };
        submit.OnHighlight += delegate { submitHL.material = usefulMats[3]; };
        submit.OnHighlightEnded += delegate { submitHL.material = usefulMats[0]; };
        submit.OnInteract += delegate { PressSubmit(); return false; };
        module.OnActivate += delegate { GenerateStage(); activated = true; };
    }
    void GenerateStage()
    {
        StopAllCoroutines();
        PickLetters();
        PickRotations();
        for (int i = 0; i < 3; i++)
        {
            questionTexts[i].text = question[i];
            StartCoroutine(RotateLetter(i));
        }
        Debug.LogFormat("[Letter Layers #{0}] The chosen letters for stage {1} are {2}.", moduleId, stage + 1, question.Join(", "));
    }
    void PickLetters()
    {
        for (int i = 0; i < 3; i++) { question[i] = allCharacters[rnd.Range(0, allCharacters.Length)].ToString(); }
        if (question[0].Equals(question[1]) || question[1].Equals(question[2]) || question[0].Equals(question[2])) { PickLetters(); }
    }
    void PickRotations()
    {
        rotationChoices = rotationChoices.Shuffle();
        for (int i = 0; i < 3; i++) { rotationDirs[i] = rnd.Range(0, 2); }
    }
    void PressArrow(bool arrow, int index)
    {
        if (!solved && !striking && activated)
        {
            if (arrow)
                sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, upArrows[index].transform);
            else
                sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, downArrows[index].transform);
            switch (index)
            {
                case 0:
                    if (arrow)
                    {
                        upArrows[index].AddInteractionPunch();
                        displayIndices[0]++;
                        if (displayIndices[0] == languages.Length) { displayIndices[0] = 0; }
                    }
                    else
                    {
                        downArrows[index].AddInteractionPunch();
                        displayIndices[0]--;
                        if (displayIndices[0] == -1) { displayIndices[0] = 6; }
                    }
                    displayTexts[0].text = languages[displayIndices[0]];
                    displayIndices[1] = 0;

                    break;
                case 1:
                    if (arrow)
                    {
                        upArrows[index].AddInteractionPunch();
                        displayIndices[1]++;
                        if (displayIndices[1] == languageArrays[displayIndices[0]].Length) { displayIndices[1] = 0; }
                    }
                    else
                    {
                        downArrows[index].AddInteractionPunch();
                        displayIndices[1]--;
                        if (displayIndices[1] == -1) { displayIndices[1] = languageArrays[displayIndices[0]].Length - 1; }
                    }
                    break;
            }
            displayTexts[1].text = languageArrays[displayIndices[0]][displayIndices[1]];
        }
    }
    void PressSubmit()
    {
        if (!solved && !striking && activated)
        {
            sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submit.transform);
            submit.AddInteractionPunch();
            Debug.LogFormat("[Letter Layers #{0}] You submitted {1}.", moduleId, answerText.text.Equals("") ? "nothing" : answerText.text);
            char[] processing = answerText.text.ToCharArray();
            string[] answer = new string[3];
            for (int i = 0; i < processing.Length; i++) answer[i] = processing[i].ToString();
            bool[] equals = new bool[3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (question[i].Equals(answer[j]))
                    {
                        equals[i] = true;
                        break;
                    }
                }
            }
            if (!equals.Contains(false))
            {
                LEDs[stage].material = usefulMats[1];
                stage++;
                if (stage == 3)
                {
                    sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                    solved = true;
                    StartCoroutine(SolveFlash());
                    module.HandlePass();
                    Debug.LogFormat("[Letter Layers #{0}] That was correct. Module solved.", moduleId);
                }
                else
                {
                    Debug.LogFormat("[Letter Layers #{0}] That was correct. Advancing to the next stage.", moduleId);
                    GenerateStage();
                }
            }
            else
            {
                module.HandleStrike();
                StartCoroutine(StrikeFlash());
            }
            answerText.text = "";
        }
    }
    IEnumerator StrikeFlash()
    {
        striking = true;
        int count = 0;
        while (count < 3)
        {
            yield return new WaitForSeconds(0.2f);
            LEDs[stage].material = usefulMats[2];
            yield return new WaitForSeconds(0.2f);
            LEDs[stage].material = usefulMats[0];
            count++;
        }
        Debug.LogFormat("[Letter Layers #{0}] That was incorrect. Strike!", moduleId);
        GenerateStage();
        striking = false;
        yield break;
    }
    IEnumerator SolveFlash()
    {
        int count = 0;
        while (count < 3)
        {
            yield return new WaitForSeconds(0.6f);
            foreach (MeshRenderer i in LEDs)
            {
                i.material = usefulMats[1];
            }
            yield return new WaitForSeconds(0.6f);
            foreach (MeshRenderer i in LEDs)
            {
                i.material = usefulMats[0];
            }
            count++;
        }
        yield return new WaitForSeconds(0.3f);
        foreach (MeshRenderer i in LEDs)
        {
            i.material = usefulMats[1];
        }
        foreach (TextMesh i in questionTexts)
        {
            i.text = "";
        }
        StopAllCoroutines();
        yield break;
    }
    IEnumerator RotateLetter(int index)
    {
        while (true)
        {
            float t = 0f;
            int rotation = 0;
            while (rotation != 90)
            {
                while (t < 0.006f)
                {
                    yield return null;
                    t += Time.deltaTime;
                }
                t = 0f;
                if (rotationDirs[index] == 1)
                    questionTexts[index].transform.Rotate(0.0f, 0.0f, rotationChoices[index], Space.Self);
                else
                    questionTexts[index].transform.Rotate(0.0f, 0.0f, -rotationChoices[index], Space.Self);
                rotation++;
            }
        }
    }
    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} submit δヨぎ [Submits the three specified letters]";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                if (parameters[1].Length != 3)
                {
                    yield return "sendtochaterror Please include only three letters to submit!";
                    yield break;
                }
                for (int i = 0; i < 3; i++)
                {
                    if (!allCharacters.Contains(parameters[1][i]))
                    {
                        yield return "sendtochaterror!f The specified letter '" + parameters[1][i] + "' is invalid!";
                        yield break;
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    int ct1 = 0, ct2 = 0;
                    int curIndex = displayIndices[0], curIndex2 = displayIndices[0];
                    while (!languageArrays[curIndex].Contains(parameters[1][0].ToString()) && !languageArrays[curIndex].Contains(parameters[1][1].ToString()) && !languageArrays[curIndex].Contains(parameters[1][2].ToString()))
                    {
                        ct1++;
                        curIndex++;
                        if (curIndex > 6)
                            curIndex = 0;
                    }
                    while (!languageArrays[curIndex2].Contains(parameters[1][0].ToString()) && !languageArrays[curIndex2].Contains(parameters[1][1].ToString()) && !languageArrays[curIndex2].Contains(parameters[1][2].ToString()))
                    {
                        ct2++;
                        curIndex2--;
                        if (curIndex2 < 0)
                            curIndex2 = 6;
                    }
                    if (ct1 < ct2)
                    {
                        for (int j = 0; j < ct1; j++)
                        {
                            upArrows[0].OnInteract();
                            yield return new WaitForSeconds(0.1f);
                        }
                        int ct3 = 0, ct4 = 0;
                        int curIndex3 = displayIndices[1], curIndex4 = displayIndices[1];
                        while (!languageArrays[curIndex][curIndex3].Equals(parameters[1][0].ToString()) && !languageArrays[curIndex][curIndex3].Equals(parameters[1][1].ToString()) && !languageArrays[curIndex][curIndex3].Equals(parameters[1][2].ToString()))
                        {
                            ct3++;
                            curIndex3++;
                            if (curIndex3 > languageArrays[curIndex].Length - 1)
                                curIndex3 = 0;
                        }
                        while (!languageArrays[curIndex][curIndex4].Equals(parameters[1][0].ToString()) && !languageArrays[curIndex][curIndex4].Equals(parameters[1][1].ToString()) && !languageArrays[curIndex][curIndex4].Equals(parameters[1][2].ToString()))
                        {
                            ct4++;
                            curIndex4--;
                            if (curIndex4 < 0)
                                curIndex4 = languageArrays[curIndex].Length - 1;
                        }
                        if (ct3 < ct4)
                        {
                            for (int j = 0; j < ct3; j++)
                            {
                                upArrows[1].OnInteract();
                                yield return new WaitForSeconds(0.1f);
                            }
                            if (languageArrays[curIndex][curIndex3].Equals(parameters[1][0].ToString()))
                            {
                                parameters[1] = parameters[1].Remove(0, 1);
                                parameters[1] = parameters[1].Insert(0, " ");
                            }
                            else if (languageArrays[curIndex][curIndex3].Equals(parameters[1][1].ToString()))
                            {
                                parameters[1] = parameters[1].Remove(1, 1);
                                parameters[1] = parameters[1].Insert(1, " ");
                            }
                            else
                            {
                                parameters[1] = parameters[1].Remove(2, 1);
                                parameters[1] = parameters[1].Insert(2, " ");
                            }
                        }
                        else if (ct3 > ct4)
                        {
                            for (int j = 0; j < ct4; j++)
                            {
                                downArrows[1].OnInteract();
                                yield return new WaitForSeconds(0.1f);
                            }
                            if (languageArrays[curIndex][curIndex4].Equals(parameters[1][0].ToString()))
                            {
                                parameters[1] = parameters[1].Remove(0, 1);
                                parameters[1] = parameters[1].Insert(0, " ");
                            }
                            else if (languageArrays[curIndex][curIndex4].Equals(parameters[1][1].ToString()))
                            {
                                parameters[1] = parameters[1].Remove(1, 1);
                                parameters[1] = parameters[1].Insert(1, " ");
                            }
                            else
                            {
                                parameters[1] = parameters[1].Remove(2, 1);
                                parameters[1] = parameters[1].Insert(2, " ");
                            }
                        }
                        else
                        {
                            for (int j = 0; j < ct3; j++)
                            {
                                downArrows[1].OnInteract();
                                yield return new WaitForSeconds(0.1f);
                            }
                            if (languageArrays[curIndex][curIndex3].Equals(parameters[1][0].ToString()))
                            {
                                parameters[1] = parameters[1].Remove(0, 1);
                                parameters[1] = parameters[1].Insert(0, " ");
                            }
                            else if (languageArrays[curIndex][curIndex3].Equals(parameters[1][1].ToString()))
                            {
                                parameters[1] = parameters[1].Remove(1, 1);
                                parameters[1] = parameters[1].Insert(1, " ");
                            }
                            else
                            {
                                parameters[1] = parameters[1].Remove(2, 1);
                                parameters[1] = parameters[1].Insert(2, " ");
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < ct2; j++)
                        {
                            downArrows[0].OnInteract();
                            yield return new WaitForSeconds(0.1f);
                        }
                        int ct3 = 0, ct4 = 0;
                        int curIndex3 = displayIndices[1], curIndex4 = displayIndices[1];
                        while (!languageArrays[curIndex2][curIndex3].Equals(parameters[1][0].ToString()) && !languageArrays[curIndex2][curIndex3].Equals(parameters[1][1].ToString()) && !languageArrays[curIndex2][curIndex3].Equals(parameters[1][2].ToString()))
                        {
                            ct3++;
                            curIndex3++;
                            if (curIndex3 > languageArrays[curIndex2].Length - 1)
                                curIndex3 = 0;
                        }
                        while (!languageArrays[curIndex2][curIndex4].Equals(parameters[1][0].ToString()) && !languageArrays[curIndex2][curIndex4].Equals(parameters[1][1].ToString()) && !languageArrays[curIndex2][curIndex4].Equals(parameters[1][2].ToString()))
                        {
                            ct4++;
                            curIndex4--;
                            if (curIndex4 < 0)
                                curIndex4 = languageArrays[curIndex2].Length - 1;
                        }
                        if (ct3 < ct4)
                        {
                            for (int j = 0; j < ct3; j++)
                            {
                                upArrows[1].OnInteract();
                                yield return new WaitForSeconds(0.1f);
                            }
                            if (languageArrays[curIndex2][curIndex3].Equals(parameters[1][0].ToString()))
                            {
                                parameters[1] = parameters[1].Remove(0, 1);
                                parameters[1] = parameters[1].Insert(0, " ");
                            }
                            else if (languageArrays[curIndex2][curIndex3].Equals(parameters[1][1].ToString()))
                            {
                                parameters[1] = parameters[1].Remove(1, 1);
                                parameters[1] = parameters[1].Insert(1, " ");
                            }
                            else
                            {
                                parameters[1] = parameters[1].Remove(2, 1);
                                parameters[1] = parameters[1].Insert(2, " ");
                            }
                        }
                        else if (ct3 > ct4)
                        {
                            for (int j = 0; j < ct4; j++)
                            {
                                downArrows[1].OnInteract();
                                yield return new WaitForSeconds(0.1f);
                            }
                            if (languageArrays[curIndex2][curIndex4].Equals(parameters[1][0].ToString()))
                            {
                                parameters[1] = parameters[1].Remove(0, 1);
                                parameters[1] = parameters[1].Insert(0, " ");
                            }
                            else if (languageArrays[curIndex2][curIndex4].Equals(parameters[1][1].ToString()))
                            {
                                parameters[1] = parameters[1].Remove(1, 1);
                                parameters[1] = parameters[1].Insert(1, " ");
                            }
                            else
                            {
                                parameters[1] = parameters[1].Remove(2, 1);
                                parameters[1] = parameters[1].Insert(2, " ");
                            }
                        }
                        else
                        {
                            for (int j = 0; j < ct3; j++)
                            {
                                downArrows[1].OnInteract();
                                yield return new WaitForSeconds(0.1f);
                            }
                            if (languageArrays[curIndex2][curIndex3].Equals(parameters[1][0].ToString()))
                            {
                                parameters[1] = parameters[1].Remove(0, 1);
                                parameters[1] = parameters[1].Insert(0, " ");
                            }
                            else if (languageArrays[curIndex2][curIndex3].Equals(parameters[1][1].ToString()))
                            {
                                parameters[1] = parameters[1].Remove(1, 1);
                                parameters[1] = parameters[1].Insert(1, " ");
                            }
                            else
                            {
                                parameters[1] = parameters[1].Remove(2, 1);
                                parameters[1] = parameters[1].Insert(2, " ");
                            }
                        }
                    }
                    deliver.OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                submit.OnInteract();
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the three letters you wish to submit!";
            }
            yield break;
        }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        while (!activated || striking) yield return true;
        int start = stage;
        for (int i = start; i < 3; i++)
            yield return ProcessTwitchCommand("submit " + question.Join(""));
    }
}