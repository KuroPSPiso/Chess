using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Pawn : MonoBehaviour
    {
        private int pawnID = -1;
        public PawnType pawnType;
        public CheckSpace checkSpace = null;
        private Color colour = Color.white;

        public GameObject modelOfType;

        public void SetPawnID(int pawnID)
        {
            this.pawnID = pawnID;
        }

        public int GetPawnID()
        {
            return this.pawnID;
        }

        public Color GetColour()
        {
            return this.colour;
        }

        public void UpdateLocation(CheckSpace checkSpace)
        {
            if(this.checkSpace != null)
            {
                this.checkSpace.changeOccupation(false);
            }

            this.gameObject.transform.position = new Vector3(
                checkSpace.Position.X,
                this.gameObject.transform.position.y,
                checkSpace.Position.Y
            );

            this.checkSpace = checkSpace;
            this.checkSpace.changeOccupation(true);
        }

        public void UpdateColour(Color colour, Material material)
        {
            Renderer renderer = this.gameObject.GetComponent<Renderer>();
            renderer.material = material;
			
			renderer = this.modelOfType.GetComponent<Renderer>();
            renderer.material = material;

            this.colour = colour;
        }

        public void UpgradeUnit(PawnType newPawnType)
        {
            Pawn.UpgradeUnit(newPawnType, this);
        }

        public static void UpgradeUnit(PawnType newPawnType, Pawn pawn)
        {
            MeshFilter meshFilter = pawn.modelOfType.GetComponent<MeshFilter>();

            MeshFilterManager mfm = GameObject.FindObjectOfType<MeshFilterManager>();
            switch(newPawnType)
            {
                case PawnType.Rook:
                    meshFilter.mesh = mfm.rookMesh.mesh;
                    break;
                case PawnType.Bischop:
                    meshFilter.mesh = mfm.bischopMesh.mesh;
                    break;
                case PawnType.Knight:
                    meshFilter.mesh = mfm.knightMesh.mesh;
                    break;
                case PawnType.Queen:
                    meshFilter.mesh = mfm.queenMesh.mesh;
                    break;
            }

            pawn.pawnType = PawnType.Queen;
        }

        [ContextMenu("Test Upgrade")]
        public void TestUpgrade()
        {
            if(this.pawnType.Equals(PawnType.Pawn))
            {
                this.UpgradeUnit(PawnType.Queen);
            }
        }
    }
}
