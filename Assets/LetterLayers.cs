using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;
public class LetterLayers : MonoBehaviour {
    public KMSelectable[] upArrows;
    public KMSelectable[] downArrows;
    public KMSelectable deliver;
    public KMSelectable submit;
    public TextMesh[] displayTexts;
    public TextMesh[] questionTexts;
    public TextMesh answerText;
    public MeshRenderer[] LEDs;
    public MeshRenderer sumbitHL;
    public Material[] usefulMats;
    string[] languages = new string[] { "E", "Gr", "K", "R", "T", "JK", "JH" };
    string allCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZαβγδεζηθικλµνξοπρστυφχψωㄱㄲㄴㄷㄹㅁㅂㅅㅆㅇㅈㅊㅋㅌㅍㅎㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣабвгдеёжзийклмнопрстуфхцчшщъыьэюяกขฃคฅงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรลวศษสหฬฃฮァイウエオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモヤユヨラリルレロワヰヱヲンヴヷヸヹヺあいうえおかがきぎくぐけげこごさざしじすずせぜそぞただちぢつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもやゆよらりるれろわゐゑをんゔ";
    string[][] languageArrays = new string[][]{
    new string[]{"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"},
    new string[]{"α","β","γ","δ","ε","ζ","η","θ","ι","κ","λ","µ","ν","ξ","ο","π","ρ","σ","τ","υ","φ","χ","ψ","ω"},
    new string[]{"ㄱ","ㄲ","ㄴ","ㄷ","ㄹ","ㅁ","ㅂ","ㅅ","ㅆ","ㅇ","ㅈ","ㅊ","ㅋ","ㅌ","ㅍ","ㅎ","ㅏ","ㅐ","ㅑ","ㅒ","ㅓ","ㅔ","ㅕ","ㅖ","ㅗ","ㅘ","ㅙ","ㅚ","ㅛ","ㅜ","ㅝ","ㅞ","ㅟ","ㅠ","ㅡ","ㅢ","ㅣ"},
    new string[]{"а","б","в","г","д","е","ё","ж","з","и","й","к","л","м","н","о","п","р","с","т","у","ф","х","ц","ч","ш","щ","ъ","ы","ь","э","ю","я"},
    new string[]{"ก","ข","ฃ","ค","ฅ","ง","จ","ฉ","ช","ซ","ฌ","ญ","ฎ","ฏ","ฐ","ฑ","ฒ","ณ","ด","ต","ถ","ท","ธ","น","บ","ป","ผ","ฝ","พ","ฟ","ภ","ม","ย","ร","ล","ว","ศ","ษ","ส","ห","ฬ","ฃ","ฮ"},
    new string[]{"ァ","イ","ウ","エ","オ","カ","ガ","キ","ギ","ク","グ","ケ","ゲ","コ","ゴ","サ","ザ","シ","ジ","ス","ズ","セ","ゼ","ソ","ゾ","タ","ダ","チ","ヂ","ツ","ヅ","テ","デ","ト","ド","ナ","ニ","ヌ","ネ","ノ","ハ","バ","パ","ヒ","ビ","ピ","フ","ブ","プ","ヘ","ベ","ペ","ホ","ボ","ポ","マ","ミ","ム","メ","モ","ヤ","ユ","ヨ","ラ","リ","ル","レ","ロ","ワ","ヰ","ヱ","ヲ","ン","ヴ","ヷ","ヸ","ヹ","ヺ"},
    new string[]{"あ","い","う","え","お","か","が","き","ぎ","く","ぐ","け","げ","こ","ご","さ","ざ","し","じ","す","ず","せ","ぜ","そ","ぞ","た","だ","ち","ぢ","つ","づ","て","で","と","ど","な","に","ぬ","ね","の","は","ば","ぱ","ひ","び","ぴ","ふ","ぶ","ぷ","へ","べ","ぺ","ほ","ぼ","ぽ","ま","み","む","め","も","や","ゆ","よ","ら","り","る","れ","ろ","わ","ゐ","ゑ","を","ん","ゔ"}};
    string[] question = new string[3];
    int[] rotations = new int[3];
    int[] displayIndices = new int[2];
    int stage;
    public KMBombModule module;
    public KMAudio sound;
    int moduleId;
    static int moduleIdCounter = 1;
    bool solved;

    void Awake() {
        moduleId = moduleIdCounter++;
        for (int i = 0; i < 2; i++)
        {
            int j = i;
            upArrows[i].OnInteract += delegate { PressArrow(true, j); return false; };
            downArrows[i].OnInteract += delegate { PressArrow(false, j); return false; };
        }
        deliver.OnInteract += delegate { deliver.AddInteractionPunch(); sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform); if (!solved && answerText.text.Length < 3) answerText.text += (displayTexts[1].text); return false; };
        submit.OnHighlight += delegate { sumbitHL.material = usefulMats[3]; };
        submit.OnHighlightEnded += delegate { sumbitHL.material = usefulMats[0]; };
        submit.OnInteract += delegate { PressSubmit(); return false; };
        module.OnActivate += delegate { GenerateStage(); };
    }
    void GenerateStage() {
        PickLetters();
        PickRotations();
        for (int i = 0; i < 3; i++)
        {
            questionTexts[i].text = question[i];
            questionTexts[i].transform.localEulerAngles = new Vector3(90, 0, rotations[i] * 5);
        }
        Debug.LogFormat("[Letter Layers #{0}] The chosen letters for stage {1} are {2}.", moduleId, stage + 1, question.Join(" ,"));
    }
    void PickLetters()
    {
        for (int i = 0; i < 3; i++) { question[i] = allCharacters[rnd.Range(0, allCharacters.Length)].ToString(); }
        if (question[0].Equals(question[1]) || question[1].Equals(question[2]) || question[0].Equals(question[2])) { PickLetters(); }
    }
    void PickRotations()
    {
        for (int i = 0; i < 3; i++) { rotations[i] = rnd.Range(0, 72); }
        if (rotations[0] == rotations[1] || rotations[1] == rotations[2] || rotations[0] == rotations[2]) { PickRotations(); }
    }
    void PressArrow(bool arrow, int index)
    {
        if (!solved)
        {
            sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            switch (index) {
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
                        if (displayIndices[1] == -1) { displayIndices[1] = languageArrays[displayIndices[0]].Length -1; }
                    }
                    break;
            }
            displayTexts[1].text = languageArrays[displayIndices[0]][displayIndices[1]];
        }
    }
    void PressSubmit()
    {
        if (!solved)
        {
            sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            submit.AddInteractionPunch();
            Debug.LogFormat("[Letter Layers #{0}] You submitted {1}.", moduleId, answerText.text);
            char[] proccesing = answerText.text.ToCharArray();
            string[] answer = new string[3];
            for (int i = 0; i < 3; i++) answer[i] = proccesing[i].ToString();
            bool equals = false;
            foreach (string i in answer) if (question.Contains(i)) equals = true;
            foreach (string i in question) if (answer.Contains(i)) equals = true;
            if (equals)
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
        yield break;
    }
}
