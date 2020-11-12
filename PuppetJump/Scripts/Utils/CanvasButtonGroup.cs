using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuppetJump.Objs
{
    public class CanvasButtonGroup : MonoBehaviour
    {
        [System.Serializable]
        public class GroupSize
        {
            public float width;
            public float height;
        }
        public GroupSize groupSize;         // the pixel dimensions of the group
        public int numRows = 1;             // how many rows in the group
        public int numColumns = 1;          // how many columns in the group
        [System.Serializable]
        public class GroupMargins
        {
            public float top;
            public float bottom;
            public float left;
            public float right;
        }
        public GroupMargins groupMargins;   // the open space around the group
        [System.Serializable]
        public class GroupGutters
        {
            public float row;
            public float col;
        }
        public GroupGutters groupGutters;   // the open space between rows and columns
        public Button buttonPrefab;         // the prefab for all buttons
        [HideInInspector]
        public List<Button> buttons = new List<Button>();

        private void Awake()
        {
            SetUpGroup();
        }

        /// <summary>
        /// Sets up the group of buttons
        /// </summary>
        public virtual void SetUpGroup()
        {
            float buttonWidth = (groupSize.width - (groupMargins.left + groupMargins.right) - ((numColumns - 1) * groupGutters.col)) / numColumns;
            float buttonHeight = (groupSize.height - (groupMargins.top + groupMargins.bottom) - ((numRows - 1) * groupGutters.row)) / numRows;

            for(int r=0;r< numRows; r++)
            {
                for(int c = 0; c < numColumns; c++)
                {
                    // get the positionn of the new button
                    Vector3 pos = new Vector3((groupMargins.left + (c * groupGutters.col) + (c * buttonWidth)), -(groupMargins.top + (r * groupGutters.row) + (r * buttonHeight)), 0f);
                    // add the button
                    Button newButton = GameObject.Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, this.transform);
                    newButton.name = (r + "_" + c).ToString();
                    // set the position of the new button
                    newButton.GetComponent<RectTransform>().localPosition = pos;
                    // set the rotation
                    newButton.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
                    // set the width and height of the button
                    newButton.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonWidth, buttonHeight);
                    // set layer
                    newButton.gameObject.layer = this.transform.gameObject.layer;

                    // if there is a box collider on the button
                    if (newButton.GetComponent<BoxCollider>())
                    {
                        newButton.GetComponent<BoxCollider>().size = new Vector3(buttonWidth, buttonHeight, buttonPrefab.GetComponent<BoxCollider>().size.z);
                        newButton.GetComponent<BoxCollider>().center = new Vector3(buttonWidth*0.5f, -(buttonHeight*0.5f), buttonPrefab.GetComponent<BoxCollider>().center.z);
                    }

                    buttons.Add(newButton);
                }
            }
        }
    }
}
