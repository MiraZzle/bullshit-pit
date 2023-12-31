﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

#nullable enable


public class DebateManager : MonoBehaviour {
    private (Question, Candidate)[] _questions;

    private int _questionNum = 0;
    private int _questionsInTotal;

    string _language;


    [SerializeField]
    private ScaleBarManager _votingBar;

    private void ShowVotingBar() {
        _votingBar.gameObject.SetActive(true);
    }
    private void HideVotingBar() {
        _votingBar.gameObject.SetActive(false);
    }

    public int PlayerAuthenticity => _player.Authenticity;
    public int EnemyAuthenticity => _enemy.Authenticity;
    public int MinAuthenticity => (int)(0.15f * Candidate.MaxAuthenticity);
    public int PlayerVoters { get; private set; } = 50;

    private void ChangePlayerVoters(int deltaVolici) {
        PlayerVoters = Mathf.Clamp(PlayerVoters + deltaVolici, 0, 100);
        _votingBar.UpdateSlider(PlayerVoters);
    }
    private void ChangeEnemyVoters(int deltaVolici) {
        ChangePlayerVoters(-deltaVolici);
    }

    [SerializeField]
    private GameObject _playerObject;
    private Candidate _player;

    [SerializeField]
    private GameObject _enemyObject;
    private Candidate _enemy;

    void Start() {
        _player = _playerObject.GetComponent<Candidate>();
        _enemy = _enemyObject.GetComponent<Candidate>();
        _language = PlayerPrefs.GetString("language");
        HideVotingBar();
    }

    public void ShowBars() {
        ShowVotingBar();
        _player.ShowAuthenticityBar();
        _enemy.ShowAuthenticityBar();
    }

    public void HideBars() {
        HideVotingBar();
        _player.HideAuthenticityBar();
        _enemy.HideAuthenticityBar();
    }

    public void SetUpQuestions() {
        var _generalQuestions = QuestionLoader.GetGeneralQuestions();             // 4 questions for both
        var _playerQuestions = QuestionLoader.GetQuestionsForCandidate(_player);  // 3 questions for player
        var _enemyQuestions = QuestionLoader.GetQuestionsForCandidate(_enemy);    // 3 questions for enemy
        var _finalQuestion = QuestionLoader.GetFinalQuestion();
        _questions = new (Question, Candidate)[] {
            // round 1
            (_generalQuestions[0], _player), (_generalQuestions[0], _enemy),
            (_playerQuestions[0], _player), (_enemyQuestions[0], _enemy),
            // round2
            (_generalQuestions[1], _player), (_generalQuestions[1], _enemy),
            (_playerQuestions[1], _player), (_enemyQuestions[1], _enemy),
            // round 3
            (_generalQuestions[2], _player), (_generalQuestions[2], _enemy),
            (_playerQuestions[2], _player), (_enemyQuestions[2], _enemy),
            (_generalQuestions[3], _player), (_generalQuestions[3], _enemy),
            // final question 
            (_finalQuestion, _player), (_finalQuestion, _enemy)
        };
        _questionsInTotal = _questions.Length;
    }

    private Question _lastQuestion;
    private Candidate _lastCandidate;
    private Answer _lastAnswer;

    public (Question?, Candidate?) AskAnotherQuestion() {
        if (_questionNum >= _questionsInTotal)
            return (null, null);

        (_lastQuestion, _lastCandidate) = _questions[_questionNum++];
        return (_lastQuestion, _lastCandidate);
    }

    public string GetIntroText() {
        string introCS = "Dámy a pánové, vítejte u prezidentské debaty, klíčového okamžiku v historii našeho národa. Dnes večer představí kandidáti " + _player.Name + " a " + _enemy.Name + " různé vize naší budoucnosti. Děkujeme vám, že jste se rozhodli státi svědky tohoto zásadního rozhovoru.";
        string introEN = "Ladies and gentlemen, welcome to the presidential debate, a pivotal moment in our nation's journey. Tonight, our candidates " + _player.Name + " and " + _enemy.Name + " present diverse visions for our future. Thank you for joining this critical conversation.";
        return (_language == "english") ? introEN : introCS;
    }

    public string GetPlayerIntroText() {
        string introCS = _player.Name + ", seriózní kandidát s vtipným odstupem k politice. Vypadá, jako by každou chvíli přednášel důležitou tezi. Jeho oblíbeným heslem je: „Rozhodnutí je jako dobrý vtip - potřebuje čas a správnou pointu.“";
        string introEN = _player.Name + ", a seasoned politician, has more political baggage than a 10-term senator at an airport carousel. Critics say they navigate issues with all the agility of a sloth in a speed-eating contest.";
        return (_language == "english") ? introEN : introCS;
    }

    public string GetEnemyIntroText() {
        string introCS = _enemy.Name + ", charismatický kandidát schopný prodat lednici Eskymákovi. Jeho kampaň je plná energie a humoru a jeho politické návrhy mají tendenci obsahovat smích, ale občas je těžké rozeznat, zda chce zlepšit stát nebo natočit sitcom.";
        string introEN = _enemy.Name + ", a private sector enthusiast, brings as much political experience as a goldfish in a game of chess – but hey, who needs political know-how when you've got a dynamic PowerPoint presentation?";
        return (_language == "english") ? introEN : introCS;
    }

    public string GetStartQuestionsIntroText() {
        string introEN = "Well, let's stop stalling and get down to what everyone is waiting for – the questions.";
        string introCS = "Přestaňme otálet a přejděme k tomu, na co všichni čekají – k otázkám.";
        return (_language == "english") ? introEN : introCS;
    }

    public string GetKickOutOfDebateText(Candidate candidate) {
        string candidateLastName = candidate.Name.Split(' ')[1];

        string textEN = "Mr " + candidateLastName + ", in this debate we stress the importance of transparency and the truthful presentation of information. Unfortunately, because of your repeated false and extremely populist statements, we have to exclude you from the debate.";
        string textCS = "Pane " + candidateLastName + ", v rámci této debaty zdůrazňujeme důležitost transparentnosti a pravdivé prezentace informací. Kvůli opakovaným nepravdivým a extrémně populistickým prohlášením vás bohužel musíme vyloučit z debaty.";
        return (_language == "english") ? textEN : textCS;

    }

    public string GetOutroText(bool kickedOut) {
        string textNormalEN = "Ladies and gentlemen, that's all from today's presidential debate. I would like to thank both candidates for their participation and their openness in discussing key issues for our country. We hope that this debate has answered your questions. Thank you for watching.";
        string textNormalCS = "Dámy a pánové, to je pro dnešní prezidentskou debatu vše. Chtěl bych poděkovat oběma kandidátům za jejich účast a otevřenost v diskusi o klíčových otázkách pro naši zemi. Doufáme, že vám tato debata odpověděla na vaše otázky. Děkujeme vám za sledování a přejeme šťastnou volbu.";

        string textKickOutEN = "Ladies and gentlemen, due to persistent misinformation, we regretfully end this debate early to uphold the integrity of our democratic process. We apologize for any inconvenience and urge viewers to seek accurate information for informed decision-making. Thank you for your understanding.";
        string textKickOutCS = "Dámy a pánové, vzhledem k opakovaným dezinformacím bohužel musíme tuto rozpravu ukončit předčasně, abychom zachovali integritu demokratického hlasování. Omlouváme se za způsobené komplikace a děkujeme vám za pochopení.";

        if (kickedOut) return (_language == "english") ? textKickOutEN : textKickOutCS;
        else return (_language == "english") ? textNormalEN : textNormalCS;
    }

    public void ProcessAnswer(Answer answer)
    {
        _lastCandidate.ChangeAuthenticity(answer.DeltaAuthenticity);

        if (_lastCandidate == _player)
            ChangePlayerVoters(answer.DeltaVolici);
        else
            ChangeEnemyVoters(answer.DeltaVolici);
        _lastAnswer = answer;
    }

    // dont show cards when all are used and when this is the final question 
    private int _numCardsUsed = 0;
    public bool ShouldShowCards => _numCardsUsed < CardManager.NumCards && _questionNum < _questionsInTotal - 1;


    public bool DecidePlayerWin(Card card) {
        return ProcessCardAttackLegacy(card);
    }
    public void UpdateAuthenticityAndVoters(Card card, bool playerWon) {
        ProcessCardAttackLegacy(card, playerWon);
        ++_numCardsUsed;
    }

    private bool ProcessCardAttackLegacy(Card card, bool? playerWon = null)
    {
        // if the player attacked, than the last question must have been for the enemy

        bool DecideWin(float multiplier)
        {
            float r = Random.Range(0f, 1f);
            return (float)PlayerAuthenticity / (float)Candidate.MaxAuthenticity * multiplier > r;
        }
        int CalculateResult(int number, float multiplier)
        {
            return (int)Mathf.Round(number * multiplier);
        }

        float probabilityMultiplier = 1f;
        float powerMultiplier = 1f;

        bool AnswerIsCommie() {
            string[] commieStrings = new string[] {
                "Obnovíme komunismus", "obnovíme komunismus", "restore communism", "Restore communism"
            };

            foreach (string commieString in commieStrings) {
                if (_lastAnswer.Text.Contains(commieString)) return true;
            }
            return false;
        } 

        void SetProbabilityMultiplier() {
            float real = 3f;
            float neutral = 1f;
            float populist = 0.9f;
            float irelevant = 0.6f;
            float general = 1f;

            // general question
            if (_lastQuestion.Type == QuestionType.General) {
                // special case for commie card and a commie answer by the enemy
                if (card.Type == CardType.Commie && AnswerIsCommie()) {
                    probabilityMultiplier = real;
                    return;
                }
                probabilityMultiplier = general;
                return;
            }
            // personal question - irrelevant
            if (!card.IsRelevantToProperty((PropertyType)_lastQuestion.AssociatedProperty!)) {
                probabilityMultiplier = irelevant;
                return;
            }

            // personal question - relevant
            switch (_lastAnswer.Type) {
                case AnswerType.Populist:
                    probabilityMultiplier = populist;
                    break;
                case AnswerType.Neutral:
                    probabilityMultiplier = neutral;
                    break;
                case AnswerType.Real:
                    probabilityMultiplier = real;
                    break;
                default:
                    break;
            }
        }

        void SetPowerMultiplier(bool playerWon)
        {
            float playerLost = 1f;
            float irelevant = 0.5f;
            float populist = 1.5f;
            float neutral = 1f;
            float real = 0.7f;
            
            // if player lost
            if (!playerWon) {
                powerMultiplier = playerLost;
                return;
            }

            // general question
            if (_lastQuestion.Type == QuestionType.General)
            {
                // special case for commie card and a commie answer by the enemy
                if (card.Type == CardType.Commie && AnswerIsCommie()) {
                    powerMultiplier = real;
                    return;
                }

                powerMultiplier = irelevant;
                return;
            }
            // personal question - irrelevant
            if (!card.IsRelevantToProperty((PropertyType)_lastQuestion.AssociatedProperty!))
            {
                powerMultiplier = irelevant;
                return;
            }

            // personal question - relevant
            switch (_lastAnswer.Type)
            {
                case AnswerType.Populist:
                    powerMultiplier = populist;
                    break;
                case AnswerType.Neutral:
                    powerMultiplier = neutral;
                    break;
                case AnswerType.Real:
                    powerMultiplier = real;
                    break;
                default:
                    break;
            }
        }


        SetProbabilityMultiplier();

        // the first time this is called (with null) this function return who won
        if (playerWon is null) {
            return DecideWin(probabilityMultiplier);
        }
        // it should be called again after that with the information of who won
        // and it will adjust stats accordingly

        SetPowerMultiplier((bool)playerWon!);
        int loserDeltaAuth = CalculateResult(card.LoserAuthenticityDelta, powerMultiplier);
        int winnerDeltaVolici = CalculateResult(card.WinnerVotersDelta, powerMultiplier);
        if ((bool)playerWon)
        {
            _enemy.ChangeAuthenticity(loserDeltaAuth);
            ChangePlayerVoters(winnerDeltaVolici);
        }
        else
        {
            _player.ChangeAuthenticity(loserDeltaAuth);
            ChangeEnemyVoters(winnerDeltaVolici);
        }
        return (bool)playerWon;
    }
}
