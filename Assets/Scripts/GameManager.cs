using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private Player player;

        public Transform AFileRoot;
        public Transform BFileRoot;
        public Transform CFileRoot;
        public Transform DFileRoot;
        public Transform EFileRoot;
        public Transform FFileRoot;
        public Transform GFileRoot;
        public Transform HFileRoot;

        public List<Pawn> whitePawns;
        public List<Pawn> blackPawns;

        int pawnIndexCounter;
        public CheckSpace[,] cells = new CheckSpace[8,8]; //expected 64 (8x8)

        public bool isWhitesTurn = false;
        public bool isBlacksTurn = false;

        public Material matBlack;
        public Material matWhite;

        public GameObject prefabKing;
        public GameObject prefabQueen;
        public GameObject prefabBischop;
        public GameObject prefabKnight;
        public GameObject prefabRook;
        public GameObject prefabPawn;

        public bool isReadyToOperate; //get's disabled after initialisation.
        public bool isInitialised;

        public void Start()
        {
            this.player = GameObject.FindObjectOfType<Player>();
            this.pawnIndexCounter = 0;
            this.isReadyToOperate = false;
            this.isInitialised = false;

            InitiateSetup();

            this.isReadyToOperate = true;
        }

        private void InitiateSetup()
        {
            SetupCells();

            //Due to the nature of JMS this will always occur at this moment
            if (!HasActiveGame())
            {
                NewGame();
            }
        }

        private bool HasActiveGame()
        {
            bool hasActiveGame = false;

            if(!isWhitesTurn && !isBlacksTurn)
            {
                this.isWhitesTurn = false; //White always starts first - BUT WITH THE SERVER THIS IS DISABLED
            }
            return hasActiveGame;
        }

        private void NewGame()
        {
            //white
            for (int y = 0; y < 2; y++)
            { 
                for (int x = 0; x < 8; x++)
                {
                    int numberRow = y + 1;
                    int letterCollumn = x + 1;

                    GameObject pawnEntity = null;
					
                    if(numberRow == 2)
                    {
                        pawnEntity = (GameObject)GameObject.Instantiate(prefabPawn, this.cells[x, y].transform);
                    }
                    else
                    {
                        if(letterCollumn == 1 || letterCollumn == 8)
                        {
                            pawnEntity = (GameObject)GameObject.Instantiate(prefabRook, this.cells[x, y].transform);
                        }
                        else if(letterCollumn == 2 || letterCollumn == 7)
                        {
                            pawnEntity = (GameObject)GameObject.Instantiate(prefabKnight, this.cells[x, y].transform);
                        }
                        else if(letterCollumn == 3 || letterCollumn == 6)
                        {
                            pawnEntity = (GameObject)GameObject.Instantiate(prefabBischop, this.cells[x, y].transform);
                        }
						else if(letterCollumn == 4)
                        {
                            pawnEntity = (GameObject)GameObject.Instantiate(prefabQueen, this.cells[x, y].transform);
                        }
						else if(letterCollumn == 5)
                        {
                            pawnEntity = (GameObject)GameObject.Instantiate(prefabKing, this.cells[x, y].transform);
                        }
                    }
                    pawnEntity.transform.rotation = Quaternion.Euler(0, 270, 0);

                    Pawn pawn = pawnEntity.GetComponent<Pawn>();
                    pawn.SetPawnID(pawnIndexCounter);
                    pawn.UpdateLocation(this.cells[x, y]);
                    pawn.UpdateColour(Color.white, this.matWhite);
                    whitePawns.Add(pawn);
					pawn.transform.SetParent(this.transform);

                    pawnIndexCounter++;
                }
            }

            //black
			for (int y = 6; y < 8; y++)
            { 
                for (int x = 0; x < 8; x++)
                {
                    int numberRow = y + 1;
                    int letterCollumn = x + 1;

                    GameObject pawnEntity = null;
					
                    if(numberRow == 7)
                    {
                        pawnEntity = (GameObject)GameObject.Instantiate(prefabPawn, this.cells[x, y].transform);
                    }
                    else
                    {
                        if(letterCollumn == 1 || letterCollumn == 8)
                        {
                            pawnEntity = (GameObject)GameObject.Instantiate(prefabRook, this.cells[x, y].transform);
                        }
                        else if(letterCollumn == 2 || letterCollumn == 7)
                        {
                            pawnEntity = (GameObject)GameObject.Instantiate(prefabKnight, this.cells[x, y].transform);
                        }
                        else if(letterCollumn == 3 || letterCollumn == 6)
                        {
                            pawnEntity = (GameObject)GameObject.Instantiate(prefabBischop, this.cells[x, y].transform);
                        }
						else if(letterCollumn == 4)
                        {
                            pawnEntity = (GameObject)GameObject.Instantiate(prefabQueen, this.cells[x, y].transform);
                        }
						else if(letterCollumn == 5)
                        {
                            pawnEntity = (GameObject)GameObject.Instantiate(prefabKing, this.cells[x, y].transform);
                        }
                    }
                    pawnEntity.transform.rotation = Quaternion.Euler(0, 90, 0);

                    Pawn pawn = pawnEntity.GetComponent<Pawn>();
                    pawn.SetPawnID(pawnIndexCounter);
                    pawn.UpdateLocation(this.cells[x, y]);
                    pawn.UpdateColour(Color.black, this.matBlack);
                    whitePawns.Add(pawn);
					pawn.transform.SetParent(this.transform);

                    pawnIndexCounter++;
                }
            }
        }

        private void SetupCells()
        {
            int x, y;

            x = 0; //Letter A
            y = 0; //Number (1 - 8)
            foreach (Transform tChild in AFileRoot)
            {
                if (tChild != AFileRoot)
                {
                    tChild.GetComponent<CheckSpace>().initiatePoint(x, y);
                    this.cells[x, y] = tChild.GetComponent<CheckSpace>();
                    y++;
                }
            }

            x = 1; //Letter B
            y = 0; //Number (1 - 8)
            foreach (Transform tChild in BFileRoot)
            {
                if (tChild != BFileRoot)
                {
                    tChild.GetComponent<CheckSpace>().initiatePoint(x, y);
                    this.cells[x, y] = tChild.GetComponent<CheckSpace>();
                    y++;
                }
            }

            x = 2; //Letter C
            y = 0; //Number (1 - 8)
            foreach (Transform tChild in CFileRoot)
            {
                if (tChild != CFileRoot)
                {
                    tChild.GetComponent<CheckSpace>().initiatePoint(x, y);
                    this.cells[x, y] = tChild.GetComponent<CheckSpace>();
                    y++;
                }
            }

            x = 3; //Letter D
            y = 0; //Number (1 - 8)
            foreach (Transform tChild in DFileRoot)
            {
                if (tChild != DFileRoot)
                {
                    tChild.GetComponent<CheckSpace>().initiatePoint(x, y);
                    this.cells[x, y] = tChild.GetComponent<CheckSpace>();
                    y++;
                }
            }

            x = 4; //Letter E
            y = 0; //Number (1 - 8)
            foreach (Transform tChild in EFileRoot)
            {
                if (tChild != EFileRoot)
                {
                    tChild.GetComponent<CheckSpace>().initiatePoint(x, y);
                    this.cells[x, y] = tChild.GetComponent<CheckSpace>();
                    y++;
                }

            }

            x = 5; //Letter F
            y = 0; //Number (1 - 8)
            foreach (Transform tChild in FFileRoot)
            {
                if (tChild != FFileRoot)
                {
                    tChild.GetComponent<CheckSpace>().initiatePoint(x, y);
                    this.cells[x, y] = tChild.GetComponent<CheckSpace>();
                    y++;
                }
            }

            x = 6; //Letter G
            y = 0; //Number (1 - 8)
            foreach (Transform tChild in GFileRoot)
            {
                if (tChild != GFileRoot)
                {
                    tChild.GetComponent<CheckSpace>().initiatePoint(x, y);
                    this.cells[x, y] = tChild.GetComponent<CheckSpace>();
                    y++;
                }
            }

            x = 7; //Letter H
            y = 0; //Number (1 - 8)
            foreach (Transform tChild in HFileRoot)
            {
                if (tChild != HFileRoot)
                {
                    tChild.GetComponent<CheckSpace>().initiatePoint(x, y);
                    this.cells[x, y] = tChild.GetComponent<CheckSpace>();
                    y++;
                }
            }
        }

        private void Update()
        {

        }
    }
}
