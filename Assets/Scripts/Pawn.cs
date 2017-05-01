using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Pawn : MonoBehaviour
    {
        public PawnType pawnType;
        public CheckSpace checkSpace = null;
        private Color colour = Color.white;

        public GameObject modelOfType;

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
    }
}
