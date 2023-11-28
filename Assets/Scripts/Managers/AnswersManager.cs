using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AnswersManager : MonoBehaviour
{
    public GameObject AnswerPrefab;
    public GameObject Parent;

    public Action<Answer> Handler;

    private List<AnswerButton> _answers = new List<AnswerButton>();
    private VerticalLayoutGroup _alignment;
    private SoundManager soundManager;

    // Start is called before the first frame update
    void Start() { 
        _alignment = GetComponent<VerticalLayoutGroup>();
        soundManager = GameObject.FindGameObjectWithTag("sound").GetComponent<SoundManager>();
    }

    public void Show() { Parent.SetActive(true); }
    public void Hide() { Parent.SetActive(false); }

    public void SetAnswers(List<Answer> answers)
    {
        ClearAnswers();
        for (int i = 0; i < answers.Count; i++)
        {
            _answers.Add(CreateAnswer(answers[i]));
        }
    }

    public void ClearAnswers()
    {
        foreach (var answer in _answers)
        {
            Destroy(answer.gameObject);
        }

        _answers.Clear();
    }

    public void DestroyExcept(Answer answer)
    {
        foreach (var ans in _answers)
        {
            if (ans.Ans.Text != answer.Text)
            {
                Destroy(ans.gameObject);
            }
            else
            {
                var but = ans.GetComponent<Button>();
                but.interactable = false;
            }
        }
    }

    private AnswerButton CreateAnswer(Answer ans)
    {
        if (ans is null)
        {
            Debug.Log("Odpoved na ot�zku je null.");
        }

        GameObject gameobj = Instantiate(AnswerPrefab, Parent.transform);
        AnswerButton answer = gameobj.GetComponent<AnswerButton>();

        answer.SetText(ans.Text);
        answer.Ans = ans;
        answer.GetComponent<Button>().onClick.AddListener(() =>
                                                          {
                                                              soundManager.PlayMouseClickSE();
                                                              Handler?.Invoke(ans);
                                                          });

        return answer;
    }
}
