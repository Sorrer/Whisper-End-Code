﻿using System;
using Game.Dialogue.Nodes.Misc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Game.Dialogue.Nodes;
using Game.Dialogue.Nodes.Inline;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Game;
using Game.Input;

namespace Game.Dialogue {
//Handles the dialogue graph provided to it. Will display, do all the effects, conditions, etc
    public class DialogueSystem : MonoBehaviour, IGameInstance
    {
        public DialogueTyper Typer;
    
        public bool ContinueInput {
            get {
                return gameInput.InteractPressed;
            }
        }

        public DialogueGraph CurrentGraph;

        public Canvas CanvasObject;

        public DialogueChoiceButtons ChoiceButtons;

        public DialogueTransitionIn currentTransition;

        public GameInput gameInput;


        private void Awake() {
            MainInstances.Add(this);
        }

        private void Start() {
            gameInput = MainInstances.Get<GameInput>();
            if(currentPresentationCoroutine != null) StopCoroutine(currentPresentationCoroutine);
        }

        [ContextMenu("Present current graph")]
        public void PresentCurrent() {
            Present(CurrentGraph);
        }

        public void Present(DialogueGraph graph) {
            
            if(graph == null) {
                Debug.LogError("Graph to present is null! Check if it is set!");
                return;
            }
            if(IsPlaying()) {
                Debug.LogWarning("Tried to play dialogue while dialogue is still being played");
                return;
            }
            CanvasObject.enabled = true;
            graph.Play(this);
            CurrentGraph = graph;
            currentPresentationCoroutine = StartCoroutine(PresentEnumerator());
        }


        public bool IsPlaying() {
            return currentPresentationCoroutine != null;
        }

        private bool isNodeWorking = false;

        private Coroutine currentPresentationCoroutine;

        public DialogueNodeBase CurrentNodeActive;
        private IEnumerator PresentEnumerator() {
            
            Debug.Log("Starting Dialogue!");
            
            
            
            
            int currentRate = 0;

            int maxRateLimit = 50; // How many times can we loop without yielding, so if an infinite loop happens we dont freeze frame

            while (CurrentGraph.current != null) {
                currentRate++;

                CurrentNodeActive = CurrentGraph.current;

                if (currentRate > maxRateLimit) {
                    yield return null;
                    currentRate = 0;

                    Debug.LogWarning("Reached rate limit, yielding and reset limit");
                }

                var currentNode = CurrentGraph.current;
                isNodeWorking = false;

                Debug.Log("Current node " + currentNode.name);

                switch (currentNode) {
                    case DialogueNode d:
                        isNodeWorking = true;
                        currentRate = 0;
                        StartCoroutine(DialogueBox(d));
                        break;

                    case ChoiceNode c:
                        isNodeWorking = true;
                        currentRate = 0;
                        StartCoroutine(ChoiceBox(c));
                        break;

                    case TextEffectNode t:
                        Typer.SetTextEffect(t);
                        break;

                    case AgentNode a:
                        Typer.SetAgent(a);
                        break;

                    case PauseNode p:
                        if (isSkipping) break;
                        Debug.Log("Pausing for " + p.PauseTime);
                        float currentTime = 0;
                        while(currentTime < p.PauseTime) {
                            currentTime += Time.unscaledDeltaTime;

                            if (ContinueInput) {
                                isSkipping = true;
                                break;
                            }

                            yield return null;
                        }
                        //Incase of missed frame
                        if (ContinueInput) {
                            isSkipping = true;
                        }

                        break;
                }


                while (isNodeWorking) {
                    yield return null;
                }

                CurrentGraph.Continue();



            }
            
            Debug.Log("Finished Dialogue!");
            currentPresentationCoroutine = null;
            Typer.Play("Done");
            Typer.ResetTyper();
            CanvasObject.enabled = false;
        }




    


        private bool isSkipping = false;
        
        private IEnumerator DialogueBox(DialogueNode node) {
            Debug.Log("Playing dialogue " + node.Dialogue);
            Typer.Play(node.Dialogue, node.ErasePrevious);

            Typer.SetIndicator(true);
            while (!Typer.IsFinished()) {
                yield return null;
                if (ContinueInput || isSkipping) {
                    isSkipping = true;
                    Typer.ForceFinish();
                    break;
                }
            }

            //Skips a frame to make sure Input is refreshed
            yield return null;
            if (node.WaitForInput) {
                Typer.SetIndicator(false);
                isSkipping = false;
                //Wait for player to continue
                while (!ContinueInput) {
                    yield return null;
                }
            }

            Debug.Log("Finished");
            isNodeWorking = false;
        }

        private IEnumerator ChoiceBox(ChoiceNode node) {


            Typer.Play(node.Dialogue, node.ErasePrevious);
            Typer.SetIndicator(true);
            while (!Typer.IsFinished()) {
                yield return null;
                if (ContinueInput || isSkipping) {
                    isSkipping = true;
                    Typer.ForceFinish();
                    break;
                }
            }

            isSkipping = false;

            List<string> textChoices = new List<string>();

            foreach(var choice in node.choices) {
                textChoices.Add(choice.text);
            }

            Typer.SetIndicator(false);
            ChoiceButtons.Show(textChoices);

            int index;
            while (!ChoiceButtons.GetChoosen(out index)) {
                yield return null;
            }
            Debug.Log(index);



            Debug.Log(node.Dialogue);
            Debug.Log(node.choices[index].text);
            node.SetChoice(index);
            isNodeWorking = false;

            //Create the box after the box gets the choice
            //With thie choice the index will be set in the dialoguegraph and we can just graph.Continue;
        }



    }


}