using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using BizzyBeeGames;

namespace TwoOfAKindGame
{
    /// <summary>
    /// This script controls the game, starting it, following game progress, and finishing it with game over.
    /// </summary>
    public class TOKGameController : MonoBehaviour
    {
        [Tooltip("Match the image to the name of the image")]
        public bool matchImageToName = false;

        // Holds the current event system
        internal EventSystem eventSystem;

        [SerializeField] GameObject[] ObjectsToHide;
        [SerializeField] Text timeText;
        [SerializeField] Animator clockAnimator;
        // Defines the type of pairs we have. These are either "ImageText" or "TextText" 
        internal string pairsType = "";

        [Tooltip("A list of all the image cards in the game. The text is derived from the text inside the card")]
        [HideInInspector]
        public Sprite[] pairsImage = null;

        [Tooltip("A list of all the text cards in the game. The comparison is made based on the name of the image")]
        [HideInInspector]
        public string[] pairsText = null;

        [Tooltip("The default card button which all other card buttons are duplicated from and displayed in the cards grid")]
        public RectTransform cardObject;

        [Tooltip("A list of the colors that the cards change to between levels")]
        public Color[] cardColors;
        internal int cardColorIndex = 0;

        [Tooltip("The object that holds all the cards")]
        public RectTransform cardsGridObject;

        [Tooltip("Randomize the list of pairs so that we don't get the same pairs each time we start the game")]
        public bool randomizePairs = true;

        private float bonusPerLevel = 100;
        private float minusPerLevel = 50;

        // The current level we are on, 1 being the first level in the game
        internal int currentLevel = 1;

        // The text object that shows which level we are in. This text object should be placed inside the GameController object and named "LevelText"
        internal Text levelText;

        [Tooltip("The progress bar for mini game")]
        public GameObject ScrollBar;

        public GameObject Divider;
        public GameObject DividerLayout;

        // An array that holds all the pairs of the current level. This is used so that we can later remove them easily when the level is completed.
        internal Transform[] pairsArray;

        // The index of the current pair we are on. This increases as we advance in the levels
        internal int currentPair = 0;

        // The number of pairs left in the current level
        internal int pairsLeft = 0;

        [Tooltip("The number of pairs in the first level")]
        public int pairsCount = 4;

        [Tooltip("The number of pairs added to the game in each level")]
        public int pairsIncrease = 2;

        [Tooltip("The maximum number of pairs allowed in the game")]
        public int pairsMaximum = 8;

        
        [SerializeField] Image CloseButton;
        [SerializeField] Image TitleHeader;
        [SerializeField] Image[] SmallerCardsBase;

        // Did we select the first object in the pair, or the second?
        internal bool firstOfPair = true;

        // Holds the first object of the pair that the player selects.
        internal Transform firstObject;

        //The current score of the player
        internal float score = 0;
        internal float scoreCount = 0;

        // The score text object which displays the current score of the player. This text object should be placed inside the GameController object and named "LevelScore"
        public Text scoreText;
        internal string scoreTextPadding;

        // The highest score we got in this game
        internal float highScore = 0;

        private float time = 10;
        internal float timeLeft;

        //The canvas of the timer in the game, the UI object and its various parts
        internal GameObject timerIcon;
        internal Image timerBar;
        internal Text timerText;

        // Holds the cursor object which shows us where we are aiming when using a keyboard or gamepad
        internal RectTransform cursor;

        private List<List<QuestReward>> multipleGifts;
        private List<GiftType> multipleGiftTypes;

        // Is the game over?
        internal bool isGameOver = false;

        [Tooltip("The level of the main menu that can be loaded after the game ends")]
        public string mainMenuLevelName = "CS_StartMenu";

        public Image BackgroundImage = null;

        public Sprite darkBackground = null;
        [Tooltip("Various sounds and their source")]
        public AudioClip soundSelect;
        public AudioClip soundCorrect;
        public AudioClip soundWrong;
        public AudioClip soundTimeUp;
        public AudioClip soundLevelUp;
        public AudioClip soundGameOver;
        public AudioClip soundVictory;
        public string soundSourceTag = "Sound";
        internal GameObject soundSource;

        // The button that will restart the game after game over
        public string confirmButton = "Submit";

        // The button that pauses the game. Clicking on the pause button in the UI also pauses the game
        public string pauseButton = "Cancel";
        internal bool isPaused = false;
        internal GameObject buttonBeforePause;

        // A general use index
        internal int index = 0;

        /// <summary>
        /// Start is only called once in the lifetime of the behaviour.
        /// The difference between Awake and Start is that Start is only called if the script instance is enabled.
        /// This allows you to delay any initialization code, until it is really needed.
        /// Awake is always called before any Start functions.
        /// This allows you to order initialization of scripts
        /// </summary>
        void Start()
        {

            //PopupManager.Instance.Show("CompletedScreen");
            // Disable multitouch so that we don't tap two answers at the same time ( prevents multi-answer cheating, thanks to Miguel Paolino for catching this bug )
            Input.multiTouchEnabled = false;

            // Cache the current event system so we can position the cursor correctly
            eventSystem = UnityEngine.EventSystems.EventSystem.current;

            // Assign the score text object
            scoreTextPadding = scoreText.text;

            SetBackgroundImage();
            SetHideableObjects(true);

            bonusPerLevel = GameConfiguration.Instance.ScoreWonForCorrectMatch;
            minusPerLevel = GameConfiguration.Instance.ScoreLostForWrongMatch;
            time = GameConfiguration.Instance.MiniGameTimeInSeconds;

            // Set up the progress bar with gift images
            SetupProgressBar();

            //Update the score
            UpdateScore();

            // Assign the level text object
            if (GameObject.Find("LevelText")) levelText = GameObject.Find("LevelText").GetComponent<Text>();

            //Get the highscore for the player
            highScore = PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name + "HighScore", 0);

            //Assign the timer icon and text for quicker access
            if (GameObject.Find("TimerIcon"))
            {
                timerIcon = GameObject.Find("TimerIcon");
                if (GameObject.Find("TimerIcon/Bar")) timerBar = GameObject.Find("TimerIcon/Bar").GetComponent<Image>();
                if (GameObject.Find("TimerIcon/Text")) timerText = GameObject.Find("TimerIcon/Text").GetComponent<Text>();
            }

            // Assign the cursor object and hide it, but only for non-mobile devices
            if (transform.Find("Cursor"))
            {
                cursor = transform.Find("Cursor").GetComponent<RectTransform>();

                cursor.gameObject.SetActive(false);
            }

            //Assign the sound source for easier access
            if (GameObject.FindGameObjectWithTag(soundSourceTag)) soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);

            // Shuffle all the pairs in the game. If we have a TextText array, shuffle it instead.
            if (pairsImage.Length > 0)
            {
                // Set the pairs type
                pairsType = "Image";

                // Randomize the list of pairs
                if (randomizePairs == true) pairsImage = ShuffleImagePairs(pairsImage);

                // Create the first level, Image - Text game mode
                UpdateLevel();
            }
            else if (pairsText.Length > 0)
            {
                // Set the pairs type
                pairsType = "Text";

                // Randomize the list of pairs
                if (randomizePairs == true) pairsText = ShuffleTextPairs(pairsText);

                // Create the first level, Text - Text game mode
                UpdateLevel();
            }
            SoundManager.Instance.PlayScreenBGM("MiniGameBGM");
        }

        public void SetHideableObjects(bool enable)
        {
            foreach (GameObject item in ObjectsToHide)
            {
                item.SetActive(enable);
            }
        }

        private void SetBackgroundImage()
        {
            if(UserDataManager.Instance.IsDarkModeOn()){
                
                    BackgroundImage.sprite = darkBackground;
                    
                GameConfiguration.Instance.SetDarkModeOnPopups(CloseButton, TitleHeader, null);
                GameConfiguration.Instance.SetDarkModeOnCards(null, SmallerCardsBase);
                return;
            }
            ChapterTier data;
            int curr_level = UserDataManager.Instance.GetData("current_level");
            for (int i = 0; i < GameConfiguration.Instance.ChapterTiers.Count; i++)
            {
                data = GameConfiguration.Instance.ChapterTiers[i];
                if (curr_level >= data.min_level && curr_level < data.max_level)
                {
                    BackgroundImage.sprite = data.chapter_image;
                    break;
                }
            }
        }
List<GameObject> DividerList ;
        void SetupProgressBar()
        {
             DividerList = new List<GameObject>();
            List<ProbableRewards> RewardsForScore = GameConfiguration.Instance.RewardsForMiniGameScore;
            GameObject start_divider = Instantiate(Divider, DividerLayout.transform);

            for (int i = 0; i < RewardsForScore.Count; i++)
            {
                GameObject divider = Instantiate(Divider, DividerLayout.transform);
                divider.GetComponent<Image>().enabled = false;
                GameObject gift = divider.transform.GetChild(1).gameObject;
                gift.GetComponent<Image>().sprite = GameConfiguration.Instance.GetGiftSprite(RewardsForScore[i].giftType);
                DividerList.Add(divider);
            }

            start_divider.GetComponent<Image>().color = new Color(0.0F, 0.0F, 0.0F, 0.0F);
            start_divider.transform.GetChild(0).gameObject.SetActive(false);
            start_divider.transform.GetChild(1).gameObject.SetActive(false);
        }
        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            // Make the score count up to its current value
            // Count up to the current value
            scoreCount = (scoreCount < score) ? Mathf.Lerp(scoreCount, score, Time.deltaTime * 10) :
                Mathf.Lerp(scoreCount, score - 1, Time.deltaTime * 10);

            CheckGiftAnim();
            // Update the score text
            UpdateScore();

            //If the game is over, listen for the Restart and MainMenu buttons
            if (isGameOver == true)
            {
                //The jump button restarts the game
                if (Input.GetButtonDown(confirmButton))
                {
                    Restart();
                }

                //The pause button goes to the main menu
                if (Input.GetButtonDown(pauseButton))
                {
                    MainMenu();
                }
            }
            else
            {
                if (cursor)
                {
                    // If we use the keyboard or gamepad, keyboardControls take effect
                    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) cursor.gameObject.SetActive(true);

                    // If we move the mouse in any direction or click it, or touch the screen on a mobile device, then keyboard/gamepad controls are lost
                    if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0 || Input.GetMouseButtonDown(0) || Input.touchCount > 0) cursor.gameObject.SetActive(false);

                    // Place the cursor at the position of the selected object
                    if (eventSystem.currentSelectedGameObject) cursor.position = eventSystem.currentSelectedGameObject.GetComponent<RectTransform>().position;
                }

                // Count down the time until game over
                if (timeLeft > 0 && !isPaused)
                {
                    // Count down the time
                    timeLeft -= Time.deltaTime;
                }

                // Update the timer
                UpdateTime();
            }
        }

        int currentGiftNum = -1;
        private void CheckGiftAnim()
        {
            int temp = getRewardIndex(GameConfiguration.Instance.RewardsForMiniGameScore);

            if(temp != currentGiftNum){
                foreach (GameObject divider in DividerList)
                {
                    divider.GetComponent<Animator>().SetBool("Enable",false);
                }
                currentGiftNum = temp;
                Debug.Log("currentGiftNum");
                Debug.Log(currentGiftNum);
                for (int i = 0; i < currentGiftNum; i++)
                {
                    DividerList[i].GetComponent<Animator>().SetBool("Enable",true);   
                }
            }
        }

        /// <summary>
        /// Pause the game, and shows the pause menu
        /// </summary>
        /// <param name="showMenu">If set to <c>true</c> show menu.</param>
        public void Pause()
        {
            isPaused = true;

            // Remember the button that was selected before pausing
            if (eventSystem) buttonBeforePause = eventSystem.currentSelectedGameObject;
        }
        public void Quit()
        {
            Pause();
            PopupManager.Instance.Show("QuitScreen");
        }
        public void CloseQuitScreen()
        {
            Unpause();
        }

        /// <summary>
        /// Resume the game
        /// </summary>
        public void Unpause()
        {
            isPaused = false;

            // Select the button that we pressed before pausing
            if (eventSystem) eventSystem.SetSelectedGameObject(buttonBeforePause);
        }

        /// <summary>
        /// Selects an object for the pair. If two objects are selected, they are compared to see if they match
        /// </summary>
        /// <param name="selectedObject"></param>
        public void SelectObject(Transform selectedObject)
        {
            if (firstOfPair == true) // Selecting the first object of a pair
            {
                // Make the selected object unclickable 
                selectedObject.GetComponent<Button>().interactable = false;

                // Select the next available button for keyboard/gamepad controls
                if (eventSystem) eventSystem.SetSelectedGameObject(selectedObject.gameObject);

                // Animate the object when selected
                if (selectedObject.GetComponent<Animator>()) selectedObject.GetComponent<Animator>().Play("Select");

                firstObject = selectedObject;

                firstOfPair = false;

                //If there is a source and a sound, play it from the source
                if (soundSource)
                {
                    // Play the select sound
                    if (soundSelect) soundSource.GetComponent<AudioSource>().PlayOneShot(soundSelect);

                    // If this button has a sound assingned to it, play it
                    if (selectedObject.GetComponent<TOKPlaySound>()) selectedObject.GetComponent<TOKPlaySound>().PlaySoundCurrent();
                }

                // Play the sound associated with this card name
                if (soundSource) soundSource.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/" + selectedObject.Find("CardFront/Text").GetComponent<Text>().text));
            }
            else
            {
                // Go through all the buttons and make them unclickable
                foreach (RectTransform imageButton in cardsGridObject) imageButton.GetComponent<Button>().interactable = false;

                // Animate the object when selected
                if (selectedObject.GetComponent<Animator>()) selectedObject.GetComponent<Animator>().Play("Select");

                //If there is a source and a sound, play it from the source
                if (soundSource)
                {
                    // Play the select sound
                    if (soundSelect) soundSource.GetComponent<AudioSource>().PlayOneShot(soundSelect);

                    // If this button has a sound assingned to it, play it
                    if (selectedObject.GetComponent<TOKPlaySound>()) selectedObject.GetComponent<TOKPlaySound>().PlaySoundCurrent();
                }

                if (firstObject == selectedObject) //If we click on the same object that we already selected, unselect it and set it back to idle state ( in animation )
                {
                    if (selectedObject.GetComponent<Animator>())
                    {
                        firstObject.GetComponent<Animator>().Play("Unselect");
                        selectedObject.GetComponent<Animator>().Play("Unselect");
                    }
                }
                else // Check if the two objects match
                {
                    StartCoroutine("Compare", selectedObject);
                }

                // Play the sound associated with this card name
                if (soundSource)
                {
                    soundSource.GetComponent<AudioSource>().Stop();

                    soundSource.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/" + selectedObject.Find("CardFront/Text").GetComponent<Text>().text));
                }
            }
        }

        /// <summary>
        /// Compares two selected card objects. If they are the same we get bonus and the cards are removed from the level. If wrong, the cards are returned to face down
        /// </summary>
        /// <param name="selectedObject"></param>
        /// <returns></returns>
        IEnumerator Compare(Transform selectedObject)
        {
            yield return new WaitForSeconds(0.4f);

            // If we selected two objects, check if they match. In Image - Text mode we compare the image of the object to the name of the other object. In Text - Text mode we compare the pairs and see if they have the same firstText and secondText values
            if (firstObject.name == selectedObject.name)
            {
                // Play the correct animation
                if (selectedObject.GetComponent<Animator>())
                {
                    firstObject.GetComponent<Animator>().Play("Correct");
                    selectedObject.GetComponent<Animator>().Play("Correct");
                }

                // Add the bonus to the score
                ChangeScore(bonusPerLevel * currentLevel);

                // Make the buttons uninteractable, so we don't have to move through them in the grid
                firstObject.GetComponent<Button>().interactable = false;
                selectedObject.GetComponent<Button>().interactable = false;

                // Stop listening for a click on these objects
                firstObject.GetComponent<Button>().onClick.RemoveAllListeners();// RemoveListener(delegate { SelectObject(firstObject); });
                selectedObject.GetComponent<Button>().onClick.RemoveAllListeners();// RemoveListener(delegate { SelectObject(selectedObject); });

                //If there is a source and a sound, play it from the source
                if (soundSource && soundCorrect) soundSource.GetComponent<AudioSource>().PlayOneShot(soundCorrect);

                // One less pait to match
                pairsLeft--;

                // If there are no more pairs to match, move to the next level
                if (pairsLeft <= 0) StartCoroutine("LevelUp", 0.5f);
            }
            else // If there is no match between the two objects
            {
                // Play the wrong animation
                if (selectedObject.GetComponent<Animator>())
                {
                    firstObject.GetComponent<Animator>().Play("Wrong");
                    selectedObject.GetComponent<Animator>().Play("Wrong");
                }

                ChangeScore(minusPerLevel * -1);

                //If there is a source and a sound, play it from the source
                if (soundSource && soundWrong) soundSource.GetComponent<AudioSource>().PlayOneShot(soundWrong);
            }

            yield return new WaitForSeconds(0.3f);

            // Reset the pair check
            firstOfPair = true;

            // Go through all the buttons and make them clickable again
            foreach (RectTransform imageButton in cardsGridObject) imageButton.GetComponent<Button>().interactable = true;

            // Select one of the clickable buttons in the cards grid
            if (eventSystem)
            {
                foreach (RectTransform imageButton in cardsGridObject)
                {
                    if (imageButton.GetComponent<Button>().interactable == true) eventSystem.SetSelectedGameObject(imageButton.gameObject);
                }
            }

            // Select the next available button for keyboard/gamepad controls
            if (eventSystem) eventSystem.SetSelectedGameObject(selectedObject.gameObject);
        }

        /// <summary>
        /// Shuffles the specified sprite pairs list, and returns it
        /// </summary>
        /// <param name="s">A list of sprite pairs</param>
        Sprite[] ShuffleImagePairs(Sprite[] pairs)
        {
            // Go through all the sprite pairs and shuffle them
            for (index = 0; index < pairsImage.Length; index++)
            {
                // Hold the pair in a temporary variable
                Sprite tempPair = pairsImage[index];

                // Choose a random index from the sprite pairs list
                int randomIndex = UnityEngine.Random.Range(index, pairsImage.Length);

                // Assign a random sprite pair from the list
                pairsImage[index] = pairsImage[randomIndex];

                // Assign the temporary sprite pair to the random question we chose
                pairsImage[randomIndex] = tempPair;
            }

            return pairs;
        }

        /// <summary>
        /// Shuffles the specified pairs list, and returns it
        /// </summary>
        /// <param name="s">A list of pairs</param>
        string[] ShuffleTextPairs(string[] pairs)
        {
            // Go through all the pairs and shuffle them
            for (index = 0; index < pairsText.Length; index++)
            {
                // Hold the pair in a temporary variable
                string tempPair = pairsText[index];

                // Choose a random index from the question list
                int randomIndex = UnityEngine.Random.Range(index, pairsImage.Length);

                // Assign a random question from the list
                pairsImage[index] = pairsImage[randomIndex];

                // Assign the temporary question to the random question we chose
                pairsText[randomIndex] = tempPair;
            }

            return pairs;
        }

        /// <summary>
        /// Updates the timer text, and checks if time is up
        /// </summary>
        void UpdateTime()
        {
            // Update the time only if we have a timer icon canvas assigned
            /*if (timerIcon)
            {
                // Update the timer circle, if we have one
                if (timerBar)
                {
                    // If the timer is running, display the fill amount left. Otherwise refill the amount back to 100%
                    timerBar.fillAmount = timeLeft / time;
                }

                // Update the timer text, if we have one
                if (timerText)
                {
                    // If the timer is running, display the timer left. Otherwise hide the text
                    timerText.text = Utils.ConvertSecondsToString(Mathf.RoundToInt(timeLeft), false);
                }

                // Time's up!
                if (timeLeft <= 0)
                {
                    // Run the game over event
                    StartCoroutine(GameOver(0.5f));

                    // Play the timer icon Animator
                    if (timerIcon.GetComponent<Animation>()) timerIcon.GetComponent<Animation>().Play();

                    //If there is a source and a sound, play it from the source
                    if (soundSource && soundTimeUp) soundSource.GetComponent<AudioSource>().PlayOneShot(soundTimeUp);
                }
            }*/
            timeText.text = Utils.ConvertSecondsToString(Mathf.RoundToInt(timeLeft), false);

            if (timeLeft <= 10 && timeLeft > 0)
            {
                clockAnimator.SetBool("focus", true);
            }
            if (timeLeft <= 0)
            {

                clockAnimator.SetBool("focus", false);
                // Run the game over event
                StartCoroutine(GameOver(0.5f));

            }
        }

        /// <summary>
		/// Change the score
		/// </summary>
		/// <param name="changeValue">Change value</param>
		void ChangeScore(float changeValue)
        {
            score += changeValue;

            //Update the score
            UpdateScore();
        }

        /// <summary>
        /// Updates the score value and checks if we got to the next level
        /// </summary>
        void UpdateScore()
        {
            //Update the score text
            int current_score = Mathf.CeilToInt(scoreCount);
            if (scoreText) scoreText.text = current_score.ToString(scoreTextPadding);
            ScrollBar.GetComponent<Scrollbar>().size = current_score / (float)GameConfiguration.Instance.MaxScoreInMiniGame;
        }

        /// <summary>
		/// Levels up, and increases the difficulty of the game
		/// </summary>
		IEnumerator LevelUp(float delay)
        {
            // Animate all previous buttons out of the scene
            for (index = 0; index < cardsGridObject.childCount; index++)
            {
                if (cardsGridObject.GetChild(index) != cardObject) cardsGridObject.GetChild(index).GetComponent<Animator>().Play("Remove");
            }

            yield return new WaitForSeconds(delay);

            int current_score = Mathf.CeilToInt(scoreCount);

            // Show the next set of pairs from the list
            if ((currentPair < pairsImage.Length || currentPair < pairsText.Length)
                && current_score < GameConfiguration.Instance.MaxScoreInMiniGame)
            {
                currentLevel++;

                // Increase the number of pairs in the level
                pairsCount += pairsIncrease;

                // Limit the number of pairs to the maximum value
                pairsCount = Mathf.Clamp(pairsCount, 0, pairsMaximum);

                //If there is a source and a sound, play it from the source
                if (soundSource && soundLevelUp) soundSource.GetComponent<AudioSource>().PlayOneShot(soundLevelUp);

                // Update the level attributes based on the type of pairs we have
                if (pairsType == "Image") UpdateLevel();
                else if (pairsType == "Text") UpdateLevel();
            }
            else // Otherwise if we finished all pairs in the game, go to the Victory screen
            {
                isGameOver = true;
                ClaimMiniGameRewards();

                // Run the victory event
                //StartCoroutine(Victory(0.5f));
            }
        }

        /// <summary>
        /// Creates a new card based on the default card object
        /// </summary>
        public void CreateCard(bool showName)
        {
            // Create a new button
            RectTransform newButton = Instantiate(cardObject) as RectTransform;

            // Set the name of the card so we can compare it with its twin
            newButton.name = pairsLeft.ToString();

            // Listen for a click to change to the next stage
            newButton.GetComponent<Button>().onClick.AddListener(delegate { SelectObject(newButton); });

            // Put it inside the button grid
            newButton.transform.SetParent(cardsGridObject);

            // Set the scale to the default button's scale
            newButton.localScale = cardObject.localScale;

            // Set the position 0 local inside the parent image holder
            newButton.localPosition = Vector3.zero;

            // Show the name text of the card
            if (showName == true)
            {
                newButton.Find("CardFront/Image").GetComponent<Image>().enabled = false;
                //newButton.Find("CardFront/Image").GetComponent<Image>().sprite = pairsImage[index];

                newButton.Find("CardFront/Text").GetComponent<Text>().enabled = true;
                newButton.Find("CardFront/Text").GetComponent<Text>().text = pairsImage[index].name;

            }
            else // Show the image of the card
            {
                newButton.Find("CardFront/Image").GetComponent<Image>().enabled = true;
                newButton.Find("CardFront/Image").GetComponent<Image>().sprite = pairsImage[index];

                newButton.Find("CardFront/Text").GetComponent<Text>().enabled = false;
                newButton.Find("CardFront/Text").GetComponent<Text>().text = pairsImage[index].name;

            }

            // Select the object for keyboard/gamepad controls
            if (eventSystem) eventSystem.SetSelectedGameObject(newButton.gameObject);
        }

        /// <summary>
        /// Updates the level, showing the next set of pairs ( Used for Image - Text matching )
        /// </summary>
        public void UpdateLevel()
        {
            // Set the maximum value of the timer
            timeLeft = time;

            // Remove all previous cards except the default one
            for (index = 0; index < cardsGridObject.childCount; index++)
            {
                if (cardsGridObject.GetChild(index) != cardObject) Destroy(cardsGridObject.GetChild(index).gameObject);
            }

            // Display the current level we are on
            if (levelText) levelText.text = Lean.Localization.LeanLocalization.GetTranslationText("ROUND") + " " + (currentLevel).ToString();

            // If we have a default button assigned and we have a list of pairs, duplicate the button and display all the pairs in the grid
            if (cardObject)
            {
                // Activate the default card object while we duplicate new cards from it
                cardObject.gameObject.SetActive(true);

                // Set the color of the card front and back
                if (cardObject.Find("CardFront").GetComponent<Image>()) cardObject.Find("CardFront").GetComponent<Image>().color = cardColors[cardColorIndex];
                if (cardObject.Find("CardFront/CardBack").GetComponent<Image>()) cardObject.Find("CardFront/CardBack").GetComponent<Image>().color = cardColors[cardColorIndex];

                //Switch to the next card color
                if (cardColorIndex < cardColors.Length - 1) cardColorIndex++;
                else cardColorIndex = 0;

                // The number of pairs we need to create for this level 
                int levelPairs = currentPair + pairsCount;

                // Create the image buttons by duplicating the default one
                for (index = currentPair; index < pairsImage.Length; index++)
                {
                    // If we passed through all the pairs in the game, stop creating them
                    if (index >= levelPairs) break;

                    // Create two buttons for this image
                    CreateCard(false);

                    if (matchImageToName == true) CreateCard(true);
                    else CreateCard(false);

                    pairsLeft++;

                    currentPair++;
                }

                // Create the text buttons by duplicating the default one
                for (index = currentPair; index < pairsText.Length; index++)
                {
                    // If we passed through all the pairs in the game, stop creating them
                    if (index >= levelPairs) break;

                    // Create two buttons for this image
                    CreateCard(false);

                    if (matchImageToName == true) CreateCard(true);
                    else CreateCard(false);

                    pairsLeft++;

                    currentPair++;
                }

                // Deactivate the default card object as we don't need it anymore
                cardObject.gameObject.SetActive(false);

                // Reset the pair check. The next time we click on an object it will be the first of a pair
                firstOfPair = true;
            }

            // Scramble all available card buttons
            for (index = 1; index < cardsGridObject.childCount; index++)
            {
                if (Random.value > 0.5f) cardsGridObject.GetChild(index).SetAsFirstSibling();
                else cardsGridObject.GetChild(index).SetAsLastSibling();
            }
        }

        /// <summary>
        /// Runs the game over event and shows the game over screen
        /// </summary>
        IEnumerator GameOver(float delay)
        {
            isGameOver = true;

            yield return new WaitForSeconds(delay);

            ClaimMiniGameRewards();
        }

        private int getRewardIndex(List<ProbableRewards> rewards)
        {
            int index = 0;
            int max_score = GameConfiguration.Instance.MaxScoreInMiniGame;
            int current_score = Mathf.CeilToInt(scoreCount);

            int factor = max_score / rewards.Count;
            for (int i = rewards.Count; i > 0; i--)
            {
                if (current_score >= factor * i) { return i; }
            }
            return index;
        }

        private void ClaimMiniGameRewards()
        {
            List<ProbableRewards> rewards = GameConfiguration.Instance.RewardsForMiniGameScore;
            int reward_index = getRewardIndex(rewards);
            multipleGifts = new List<List<QuestReward>>();
            multipleGiftTypes = new List<GiftType>();

            if (reward_index > 0)
            {
                for (int i = 0; i < reward_index; i++)
                {
                    ProbableRewards reward = rewards[i];
                    List<QuestReward> allRewards = InventoryManager.Instance.GetProbableRewards(reward);
                    multipleGifts.Add(allRewards);
                    multipleGiftTypes.Add(reward.giftType);
                }
                // ClaimMultipleGifts();
            }
            PopupManager.Instance.Show("CompletedScreen", new object[] { scoreCount, currentLevel, multipleGifts, multipleGiftTypes });
        }

        public void ClaimMultipleGifts()
        {
            Debug.Log("Claim");
            Debug.Log("Item " + (multipleGifts.Count - (multipleGifts.Count - 1)) + " of " + multipleGifts.Count);
            UserDataManager.Instance.SetUserGiftReward(multipleGifts[0],
                    multipleGiftTypes[0], "MiniGameRewards");
            System.Action<object[]> giftScreenCallback = ShowNextGiftInList;

            PopupManager.Instance.Show("Blank");
            PopupManager.Instance.Show("GiftScreen", new object[] { giftScreenCallback, false });
        }
        public void ShowNextGiftInList(object[] arg)
        {
            Debug.Log("Gift Closed");
            multipleGifts.RemoveAt(0);
            multipleGiftTypes.RemoveAt(0);
            StartCoroutine(Utils.ExecuteAfterDelay(1.2f, (args) =>
            {
                Debug.Log("Next");
                Debug.Log("Item Count: " + multipleGifts.Count);
                if (multipleGifts.Count > 0) { ClaimMultipleGifts(); }
                else { MainMenu(); }
            }));
        }

        /// <summary>
        /// Restart the current level
        /// </summary>
        void Restart()
        {
            LoadingManager.Instance.LoadScene(LoadingManager.TOKGameScreen);
        }

        /// <summary>
        /// Restart the current level
        /// </summary>
        public void MainMenu()
        {
            PopupManager.Instance.CloseAllActivePopup();
            SetHideableObjects(false);
            LoadingManager.Instance.LoadScene(LoadingManager.StartScreen);
        }
    }
}