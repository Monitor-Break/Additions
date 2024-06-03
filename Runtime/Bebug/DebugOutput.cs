using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonitorBreak.Bebug
{
    public class DebugOutput
    {
        public class Section
        {
            public string name;
            private List<Part> parts = new List<Part>();

            public List<Part> GetParts()
            {
                return parts;
            }

            public float width = 100.0f;

            public Section(string name, float width = 100.0f)
            {
                this.name = name;
                this.width = width;
            }

            public void AddPart(Part newPart)
            {
                parts.Add(newPart);
            }
        }

        public class Part
        {
            public enum PartType
            {
                Text
            }

            public PartType type = PartType.Text;
            public string mainText;
            public TextAnchor anchor = TextAnchor.MiddleLeft;

            public bool active = true;

            public Part(PartType type, string mainText, TextAnchor anchor = TextAnchor.MiddleLeft)
            {
                this.type = type;
                this.mainText = mainText;
                this.anchor = anchor;
            }
        }

        public class PartHandle
        {
            public int sectionIndex;
            public int partIndex;
        }

        private List<Section> sections = new List<Section>();

        public DebugOutput(List<Section> initalSections)
        {
            sections = initalSections;
            Init();
        }

        private void Init()
        {
            BebugManagement.AddDebugOutput(this);
        }

        public void Close()
        {
            BebugManagement.RemoveDebugOutput(this);
        }

        public Vector3 DrawOutput(Vector3 startingOffset)
        {
            TextAnchor normalAnchor = GUI.skin.box.alignment;
            Vector3 drawOffset = startingOffset;

            //Draw each section
            foreach (Section section in sections)
            {
                Vector3 size = new Vector3(section.width, 0.0f);

                foreach (Part part in section.GetParts())
                {
                    if (part.active)
                    {
                        GUI.skin.box.alignment = part.anchor;

                        if (part.type == Part.PartType.Text)
                        {
                            size.y = 22.0f;

                            GUI.Box(
                                new Rect(drawOffset, size),
                                part.mainText);
                        }

                        drawOffset.y += size.y;
                    }
                }
            }

            GUI.skin.box.alignment = normalAnchor;
            return drawOffset;
        }


        public Part GetSectionPart(int sectionIndex, int partIndex)
        {
            return sections[sectionIndex].GetParts()[partIndex];
        }

        public PartHandle AddPart(int sectionIndex, Part part)
        {
            sections[sectionIndex].AddPart(part);

            PartHandle toReturn = new PartHandle();
            toReturn.sectionIndex = sectionIndex;
            toReturn.partIndex = sections[sectionIndex].GetParts().Count - 1;

            return toReturn;
        }


        //MAIN UPDATE FUNCTIONS

        public void UpdateText(PartHandle handle, string newText)
        {
            GetSectionPart(handle.sectionIndex, handle.partIndex).mainText = newText;
        }

        public void SetActive(PartHandle handle, bool active)
        {
            GetSectionPart(handle.sectionIndex, handle.partIndex).active = active;
        }
    }
}
