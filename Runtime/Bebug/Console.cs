using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace MonitorBreak.Bebug
{
    public class Console
    {
        /**Notes:
            - Backspace doesn't work on laptop?? (maybe different output)
            - Make sure view is in 16:9
            - Text input dosen't work on lower frame rate (around sub 60) (frame rate based on unity editor stats panel so not necesarily accurate)
            *! Text that goes over one line does not work very well (most of the text isn't shown)
        **/

        public Console(string name)
        {
            SetName(name);
            BebugManagement.AddConsole(this);
            currentOutputColor = new Color(0.8f, 0.8f, 0.8f);
        }

        private string consoleName;
        public void SetName(string name)
        {
            consoleName = name.ToUpper();
        }

        private static bool consoleActivated = false;

        public float scale = 1.0f;

        [TextArea(3, 10)]
        public string outputText = "";
        private const float intialTextSize = 30;
        private float textHeight = intialTextSize;
        private const int maxCharacters = 50000;
        private struct Highlight
        {
            public float displacement; //Displacement from the output
            public float size;
            public Texture2D mainTex;
        }
        private List<Highlight> highlights = new List<Highlight>();

        public string inputString = "";
        public static List<string> inputHistory = new List<string>();
        public string GetSelectedHistory()
        {
            return inputHistory[inputHistory.Count - currentIndexFromBackInHistory];
        }

        public string GetHistoryOrBaseString()
        {
            if (currentIndexFromBackInHistory > 0)
            {
                return GetSelectedHistory();
            }
            else
            {
                return inputString;
            }
        }
        private int currentIndexFromBackInHistory = 0;

        private int indexInConsoleList;

        public void SetConsoleIndex(int newIndex)
        {
            indexInConsoleList = newIndex;
        }

        public int GetConsoleIndex()
        {
            return indexInConsoleList;
        }

        //Custom commands
        public struct CustomCommand
        {
            public string identifier;
            public MethodInfo method;

            public string desc;
            public string argumentsString;
        }

        private static List<CustomCommand> customCommands = new List<CustomCommand>();

        public static void SetCustomCommands(List<CustomCommand> newCustomCommands)
        {
            customCommands = newCustomCommands;
        }
        //

        public static void Execute(string inputString, Console output = null)
        {
            //Validate string
            if (string.IsNullOrEmpty(inputString))
            {
                return;
            }
            //

            string cachedInputString = inputString;
            inputString = inputString.ToLower();

            string[] inputStringSplit = inputString.Split(' ');

            //Execute Command
            //If a custom command has been entered execute it here
            foreach(CustomCommand command in customCommands)
            {
                if(command.identifier == inputStringSplit[0])
                {
                    string[] arguments = null;
                    if(inputStringSplit.Length > 1)
                    {
                        arguments = inputStringSplit[1].Split(',');
                    }
                    command.method.Invoke(null, arguments);
                    return;
                }
            }
   
            if(inputString == "commands")
            {
                string commandsOutput = " -- Custom Commands -- \n\n";

                foreach(CustomCommand command in customCommands)
                {
                    string desc = command.desc;

                    if (string.IsNullOrEmpty(desc))
                    {
                        desc = $"Method Name is '{command.method.Name}' ";
                    }

                    commandsOutput += "   " + command.identifier.ToUpperInvariant() + $" {command.argumentsString}" + "\n      " + desc + "\n\n";
                }

                Log(commandsOutput, output.GetConsoleIndex(), false);
            }
            else if (inputString == "hide")
            {
                HideAllConsoles();
            }
            else if (inputString == "clear" || inputString[0] == '.')
            {
                output.ResetConsole();
            }
            else if (inputString == "new")
            {
                BebugManagement.MakeNewConsoleWhileInConsoleLoop();
            }
            else if (inputString == "exit")
            {
                BebugManagement.RemoveConsole(output);
                return;
            }
            else  //If there is no modifier print input string to console
            {
                Log(cachedInputString, output.GetConsoleIndex());
            }
        }

        private const float titleHeight = 20;
        private const float textInputHeight = 22;
        private const float singleLineHeight = 25;
        private const float singleTextLineHeight = 15;
        private const float scrollSpeed = 50.0f;

        private float totalScrollMovement = 0.0f;

        public enum ConsoleState
        {
            FullScreen,
            Sidebar,
            SingleLine
        }

        private static ConsoleState currentConsoleState = ConsoleState.FullScreen;

        public static ConsoleState GetConsoleState()
        {
            return currentConsoleState;
        }

        public void DrawConsole()
        {
            if (consoleActivated)
            {
                if (currentConsoleState == ConsoleState.SingleLine && indexInConsoleList != 0)
                {
                    return;
                }

                Vector2 consoleDimensions = new Vector2(Screen.width, Screen.height);
                Vector2 offset = new Vector2();

                GUIUtility.ScaleAroundPivot(Vector2.one * scale, consoleDimensions * 0.5f);
                //screenDimensions = screenDimensions * scale;

                if (currentConsoleState == ConsoleState.SingleLine)
                {
                    Rect singleLineRect = new Rect(-50, consoleDimensions.y - singleLineHeight, consoleDimensions.x + 100, singleLineHeight);
                    for (int i = 0; i < 10; i++)  //Can't change the backing colour directly so this is used to darken the backing
                    {
                        GUI.Box(singleLineRect, "");
                    }

                    singleLineRect.y -= singleLineHeight * 0.5f;
                    DrawHorizontalLine(singleLineRect);

                    //Get last line
                    int amountOfLines = outputText.Split('\n').Length;
                    if (amountOfLines > 0)
                    {
                        string singleLine = outputText.Split('\n')[amountOfLines - 1];
                        if (currentComboCounter > 1)
                        {
                            singleLine += $" x{currentComboCounter}";
                        }

                        UpdateOutputText(new Rect(5, consoleDimensions.y - singleLineHeight, consoleDimensions.x, singleLineHeight), singleLine);
                    }
                }
                else
                {
                    if (currentConsoleState == ConsoleState.Sidebar)
                    {
                        consoleDimensions = new Vector2(consoleDimensions.x * 0.3f, consoleDimensions.y / BebugManagement.NumberOfConsoles());
                        offset.y = consoleDimensions.y * ((BebugManagement.NumberOfConsoles() - 1.0f) - indexInConsoleList);
                    }
                    else
                    {
                        consoleDimensions.x /= BebugManagement.NumberOfConsoles();
                        offset.x = consoleDimensions.x * (indexInConsoleList);
                    }

                    float bottomDisplacement = textInputHeight;

                    if (currentConsoleState == ConsoleState.Sidebar)
                    {
                        bottomDisplacement = -5;    //! When text length is too long the text will be wrapped, this means text is now offscreen
                    }

                    //Backing
                    //We add 2 here to fix an issue with multiple consoles not tiling correctly, essentially the length they would get would involve a recurring decimal in the calculation
                    GUI.Box(new Rect(offset.x, offset.y, consoleDimensions.x + 2, consoleDimensions.y), "");
                    //Backing

                    float yPosition = ((consoleDimensions.y - bottomDisplacement) - textHeight) + totalScrollMovement;
                    Rect textSize = new Rect(offset.x, offset.y + yPosition, consoleDimensions.x, textHeight);

                    string finalText = outputText;
                    if (currentComboCounter > 1)
                    {
                        finalText += $" x{currentComboCounter}";
                    }

                    Rect tempCopy = textSize;

                    //Highlights
                    float lastDisplacement = -1000;

                    foreach (Highlight highlight in highlights)
                    {
                        if (highlight.displacement == lastDisplacement)
                        {
                            continue;
                        }
                        lastDisplacement = highlight.displacement;
                        textSize.y = yPosition + highlight.displacement + offset.y;
                        textSize.height = highlight.size;
                        GUI.skin.box.normal.background = highlight.mainTex;
                        GUI.Box(textSize, GUIContent.none);
                    }

                    GUI.skin.box.normal.background = BebugManagement.baseTexture;

                    UpdateOutputText(tempCopy, finalText);
                    //Highlights

                    float maxScrollDisplacement = Mathf.Clamp(textHeight - consoleDimensions.y + titleHeight + bottomDisplacement, 0, Mathf.Infinity);

                    totalScrollMovement = Mathf.Clamp(
                        totalScrollMovement + Event.current.delta.y * Time.unscaledDeltaTime * scrollSpeed,
                        0,
                        maxScrollDisplacement);
                }

                //Limit size of output
                if (outputText.Length > maxCharacters)
                {
                    int difference = outputText.Length - maxCharacters;
                    outputText = outputText.Substring(difference, maxCharacters);
                }

                if (currentConsoleState == ConsoleState.FullScreen && BebugManagement.IsActiveConsole(indexInConsoleList))
                {
                    Rect inputFieldRect = new Rect(offset.x, consoleDimensions.y - (textInputHeight), consoleDimensions.x, textInputHeight);
                    GUI.Box(inputFieldRect, "");

                    TypingControl();

                    inputFieldRect.x += 5;
                    string finalOutput = $"> {GetHistoryOrBaseString()}";
                    GUI.Label(inputFieldRect, finalOutput);

                    //Pointer
                    if (finalOutput.Length > 0)
                    {
                        int actualPointer = (int)Mathf.Clamp(GetActualPointer(), 0, Mathf.Infinity);
                        inputFieldRect.x += new GUIStyle().CalcSize(new GUIContent(finalOutput.Substring(0, actualPointer + 2))).x; //5 + width of text behind pointer
                        GUI.Label(inputFieldRect, "|");
                    }

                    inputFieldRect.y -= textInputHeight * 0.6f;
                    inputFieldRect.x = offset.x;
                    DrawHorizontalLine(inputFieldRect);
                }
                else if (currentConsoleState == ConsoleState.Sidebar)
                {
                    DrawVerticalLine(new Rect(consoleDimensions.x, -10, 10, Screen.height + 20));
                }

                if (currentConsoleState != ConsoleState.SingleLine)
                {
                    Rect titleRect = new Rect(offset.x, offset.y, consoleDimensions.x, titleHeight);
                    GUI.Box(titleRect, $"|   {consoleName}   |"); //Title
                    titleRect.y += titleHeight * 0.5f;
                    DrawHorizontalLine(titleRect);
                }

            }

            //Change state controls
            if (Event.current.keyCode == KeyCode.F1)
            {
                UpdateState(ConsoleState.FullScreen);
            }
            else if (Event.current.keyCode == KeyCode.F2)
            {
                UpdateState(ConsoleState.Sidebar);
            }
            else if (Event.current.keyCode == KeyCode.F3)
            {
                UpdateState(ConsoleState.SingleLine);
            }
            else if (Event.current.keyCode == KeyCode.F4)
            {
                HideAllConsoles();
            }
        }

        private void ResetConsole()
        {
            outputText = "";
            previousText = "reset/reset/reset/rest/"; //This is so the input and what is typed in does't match
            highlights = new List<Highlight>();
            currentComboCounter = 0;
            CalculateTextHeight();
        }

        private Color currentOutputColor = Color.white;

        private void UpdateOutputText(Rect textSize, string finalText)
        {
            GUI.color = currentOutputColor;
            GUI.Label(textSize, finalText);
            GUI.color = Color.white;
        }

        private int lineLength = 500;

        private void DrawHorizontalLine(Rect rect, char character = '─')
        {
            GUI.Label(rect, new String(character, lineLength));
        }

        private void DrawVerticalLine(Rect rect)
        {
            GUI.TextArea(rect, "");
        }

        private void AutohighlightLine(string lineText, Color color)
        {
            AddHighlight((textHeight - intialTextSize) + 3, new GUIStyle().CalcSize(new GUIContent(lineText.ToString())).y, color);
        }

        private void AddHighlight(float displacement, float size, Color color)
        {
            Highlight newHighlight = new Highlight();
            //Texture
            newHighlight.mainTex = BebugManagement.GenerateTexture(color);
            //Displacement
            newHighlight.displacement = displacement;
            //Size
            newHighlight.size = size;
            highlights.Add(newHighlight);
        }

        private char lastChar = ' ';
        private float minimumTimeTillNextChar = 0.1f;  //You can decrease this if you have a "higher" end device (anything beyond like 60 fps) (0.1 and 0.25 are good values)
        private float currentTimeTillSameChar;

        private KeyCode lastKeyCode;
        private float minimumTimeTillNextCommand = 0.3f;
        private float currentTimeTillSameCommand;

        private int pointerDistanceFromBack = 0;

        private int GetActualPointer()
        {
            return (int)Mathf.Clamp(GetHistoryOrBaseString().Length, 0, Mathf.Infinity) - pointerDistanceFromBack;
        }

        private void TypingControl()
        {
            int actualPointer = GetActualPointer();

            //Pointer control
            KeyCode inputKeyCode = Event.current.keyCode;
            if (inputKeyCode != KeyCode.None)
            {
                if (inputKeyCode != lastKeyCode || currentTimeTillSameCommand < 0)
                {
                    lastKeyCode = inputKeyCode;
                    currentTimeTillSameCommand = minimumTimeTillNextCommand;

                    int lastCIFBIH = currentIndexFromBackInHistory;
                    if (inputKeyCode == KeyCode.UpArrow)
                    {
                        currentIndexFromBackInHistory += 1;
                    }
                    else if (inputKeyCode == KeyCode.DownArrow)
                    {
                        currentIndexFromBackInHistory -= 1;
                    }

                    if (lastCIFBIH != currentIndexFromBackInHistory)
                    {
                        if (currentIndexFromBackInHistory < 0)
                        {
                            currentIndexFromBackInHistory = 0;
                        }

                        if (currentIndexFromBackInHistory > inputHistory.Count)
                        {
                            currentIndexFromBackInHistory = inputHistory.Count;
                        }

                        currentTimeTillSameCommand *= 2.5f;
                    }

                    int lastPointerValue = pointerDistanceFromBack;
                    if (inputKeyCode == KeyCode.LeftArrow)
                    {
                        pointerDistanceFromBack += 1;
                    }
                    else if (inputKeyCode == KeyCode.RightArrow)
                    {
                        pointerDistanceFromBack -= 1;
                    }

                    if (lastPointerValue != pointerDistanceFromBack)
                    {
                        if (pointerDistanceFromBack + 1 >= GetHistoryOrBaseString().Length)
                        {
                            pointerDistanceFromBack = GetHistoryOrBaseString().Length - 1;
                        }

                        if (pointerDistanceFromBack < 0)
                        {
                            pointerDistanceFromBack = 0;
                        }
                    }

                    if (Event.current.control)
                    {
                        if (inputKeyCode == KeyCode.C)
                        {
                            GUIUtility.systemCopyBuffer = GetHistoryOrBaseString();
                        }
                        else if (inputKeyCode == KeyCode.V)
                        {
                            InsertStringIntoInput(GUIUtility.systemCopyBuffer, actualPointer);
                        }
                    }
                }
            }

            if (currentTimeTillSameCommand >= 0)
            {
                currentTimeTillSameCommand -= Time.unscaledDeltaTime;
            }

            //Typing control
            string currrentKey = Input.inputString;
            if (currrentKey.Length > 0)
            {
                char character = currrentKey[0];
                if (character != lastChar || currentTimeTillSameChar < 0)
                {
                    lastChar = character;
                    currentTimeTillSameChar = minimumTimeTillNextChar;
                    if (character == '\b') //Backspace
                    {
                        if (inputString.Length != 0)
                        {
                            inputString = inputString.Remove(actualPointer - 1, 1);    //! Seems some problem with paste / maybe input history
                            if (actualPointer == 0)
                            {
                                pointerDistanceFromBack -= 1;
                            }
                        }
                    }
                    else if (character == '\n' || character == '\r') //Enter / Return
                    {
                        string finalString = GetHistoryOrBaseString();
                        Execute(GetHistoryOrBaseString(), this);
                        //Add input to history if the input isn't empty (or it is not the same as the last one in history)
                        if (!string.IsNullOrEmpty(finalString) && !IsStringRecentHistory(finalString))
                        {
                            inputHistory.Add(finalString);
                        }
                        inputString = "";
                        pointerDistanceFromBack = 0;
                        currentIndexFromBackInHistory = 0;
                    }
                    else
                    {
                        InsertStringIntoInput(character.ToString(), actualPointer);
                    }
                }
            }

            if (currentTimeTillSameChar >= 0)
            {
                currentTimeTillSameChar -= Time.unscaledDeltaTime;
            }
        }

        private bool IsStringRecentHistory(string _string)
        {
            if (inputHistory.Count == 0)
            {
                return false;
            }

            return _string == inputHistory[inputHistory.Count - 1];
        }

        private void InsertStringIntoInput(string _string, int actualPointer)
        {
            //If we are searching through input history then apply that here before anything else
            if (currentIndexFromBackInHistory > 0)
            {
                inputString = GetSelectedHistory();
                currentIndexFromBackInHistory = 0;
            }

            if (pointerDistanceFromBack == 0)  //At the last character in the word 
            {
                inputString += _string;
            }
            else
            {
                inputString = inputString.Insert(actualPointer, _string);
            }
        }

        public static void UpdateState(ConsoleState newState)
        {
            if (!BebugManagement.DebugEnabled)
            {
                return;
            }

            currentConsoleState = newState;
            if (currentConsoleState == ConsoleState.FullScreen)
            {
                Time.timeScale = 0.0f;
            }
            else
            {
                Time.timeScale = 1.0f;
            }

            consoleActivated = true;
        }

        public enum ErrorTypes
        {
            Generic,
            CannotExecute,
        }

        public static object LogError(object output, ErrorTypes errorType = ErrorTypes.Generic, int consoleNumber = 0)
        {
            // #if UNITY_EDITOR
            // output = $"{output}\n{System.Environment.StackTrace}";   //Incompatible with highlighting
            // #endif     
            Console currentConsole = BebugManagement.GetConsole(consoleNumber);
            currentConsole.OutputLine($"{errorType.ToString()} ERROR: {output}");
            currentConsole.AutohighlightLine(output.ToString(), Color.red);
            return output;
        }

        public static object Log(object output, int consoleNumber = 0, bool trace = true, int steps = 3)
        {
            string outputString = output.ToString();

#if UNITY_EDITOR
            if (trace)
            {
                outputString = $"[{GetLineFunctionWasCalledAt(steps)}] {outputString}";
            }
#endif

            BebugManagement.GetConsole(consoleNumber).OutputLine(outputString);
            return output;
        }

        private static string GetLineFunctionWasCalledAt(int stepsUp = 1)
        {
            string[] startingStackTrace = System.Environment.StackTrace.Split('\n');
            string targetLine = startingStackTrace[stepsUp];
            return targetLine.Substring(targetLine.LastIndexOf('\\') + 1).Replace(" ", "").Replace(":", ": Line ").Replace(".cs", "");
        }

        private string previousText = "";
        private int currentComboCounter = 1;

        public void OutputLine(string output)
        {
            if (output == previousText)
            {
                currentComboCounter += 1;
            }
            else
            {
                if (currentComboCounter > 1)
                {
                    OutputText($" x{currentComboCounter}");
                    currentComboCounter = 1;
                }

                if (outputText.Length > 0)
                {
                    outputText += "\n";
                }

                OutputText(output);
                previousText = output;
            }

            if (totalScrollMovement != 0)
            {
                //When new line is output add to total scroll movement if not at bottom
                totalScrollMovement += singleTextLineHeight;
            }
        }

        public void OutputText(string output)
        {
            outputText = $"{outputText}{output}";
            CalculateTextHeight();
        }

        private void CalculateTextHeight()
        {
            textHeight = (outputText.Split('\n').Length + 1) * singleTextLineHeight;
        }

        public static void HideAllConsoles()
        {
            consoleActivated = false;
            Time.timeScale = 1.0f;
        }
    }
}
