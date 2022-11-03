using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GluonGui;

namespace DialogueEditor
{
    public abstract class UINode
    {
        // Events
        public delegate void UINodeSelectedEvent(UINode node, bool selected);
        public static UINodeSelectedEvent OnUINodeSelected;

        public delegate void UINodeDeletedEvent(UINode node);
        public static UINodeDeletedEvent OnUINodeDeleted;

        public delegate void CreateSpeechEvent(UINode node);
        public static CreateSpeechEvent OnCreateSpeech;

        public delegate void ConnectToNodeEvent(UINode node);
        public static ConnectToNodeEvent OnConnect;


        // Consts
        protected const int TEXT_BORDER = 5;
        protected const int TITLE_HEIGHT = 18;
        protected const int TITLE_GAP = 4;
        protected const int TEXT_BOX_HEIGHT = 40;
        public const int LINE_WIDTH = 3;

        // Members
        public Rect rect;
        public bool isDragged;
        public bool isSelected;
        protected string title;
        protected GUIStyle currentBoxStyle;
        public Vector2 viewOffset = Vector2.zero;

        // Static
        private static GUIStyle titleStyle;
        protected static GUIStyle textStyle;

        // Properties
        public EditableConversationNode Info { get; protected set; }
        public abstract Color DefaultColor { get; }
        public abstract Color SelectedColor { get; }


        //---------------------------------
        // Constructor 
        //---------------------------------

        public UINode(EditableConversationNode infoNode, Vector2 pos)
        {
            Info = infoNode;

            if (titleStyle == null)
            {
                titleStyle = new GUIStyle();
                titleStyle.alignment = TextAnchor.MiddleCenter;
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.normal.textColor = Color.white;
            }
            if (textStyle == null)
            {
                textStyle = new GUIStyle();
                textStyle.normal.textColor = Color.white;
                textStyle.wordWrap = true;
                textStyle.stretchHeight = false;
                textStyle.clipping = TextClipping.Clip;
            }
        }

        protected void CreateRect(Vector2 pos, Vector2 viewPos, float wid, float hei)
        {
            rect = new Rect(pos.x + viewPos.x, pos.y + viewPos.y, wid, hei);
            Info.EditorInfo.xPos = rect.x - viewPos.x;
            Info.EditorInfo.yPos = rect.y - viewPos.y;
        }


        // Generic methods, called from window
        public void SetPosition(Vector2 newPos, Vector2 viewPos)
        {
            Vector2 centeredPos = new Vector2(newPos.x - rect.width / 2, newPos.y - rect.height / 2);
            rect.position = centeredPos;
            Info.EditorInfo.xPos = centeredPos.x - viewPos.x;
            Info.EditorInfo.yPos = centeredPos.y - viewPos.y;
        }




        //---------------------------------
        // Drawing
        //---------------------------------

        public void Draw(Vector2 offset)
        {
            //viewOffset = offset;

            // Box
            GUI.Box(rect, title, currentBoxStyle);

            OnDraw();
        }

        protected void DrawTitle(string text)
        {
            Rect title = new Rect(rect.x, rect.y, rect.width, TITLE_HEIGHT);
            GUI.Label(title, text, titleStyle);
        }

        protected void DrawInternalText(string text, float leftOffset = 0, float heightOffset = 0)
        {
            Rect internalText = new Rect(rect.x + TEXT_BORDER + leftOffset, 
                rect.y + TITLE_HEIGHT + TITLE_GAP + heightOffset, 
                rect.width - TEXT_BORDER * 2 - leftOffset, 
                TEXT_BOX_HEIGHT);
            GUI.Box(internalText, text, textStyle);
        }



        //---------------------------------
        // Interactions / Input Events
        //---------------------------------

        public void DragView(Vector2 moveDelta)
        {
            viewOffset += moveDelta;
            rect.position += moveDelta;
        }

        public void Drag (Vector2 moveDelta)
        {
            rect.position += moveDelta;
            Info.EditorInfo.xPos += moveDelta.x;
            Info.EditorInfo.yPos += moveDelta.y;
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                isDragged = true;
                isSelected = true;
            }
            else
            {
                isSelected = false;
            }

            OnSetSelected(selected);
        }

        public bool ProcessEvents(Event e, bool inPanel)
        {
            switch (e.type)
            {
                case UnityEngine.EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (rect.Contains(e.mousePosition) && !inPanel)
                        {
                            DialogueEditorWindow.NodeClickedOnThisUpdate = true;
                            OnUINodeSelected?.Invoke(this, true);
                            e.Use();
                        }

                        GUI.changed = true;                     
                    }
                    else if (e.button == 1 && rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }
                    break;

                case UnityEngine.EventType.MouseUp:
                    isDragged = false;
                    break;

                case UnityEngine.EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                    }
                    return true;
            }

            return false;
        }




        //---------------------------------
        // Inhereted, shared behaviour
        //---------------------------------

        protected void CreateSpeech()
        {
            OnCreateSpeech?.Invoke(this);
        }

        protected void ConnectToNode()
        {
            OnConnect?.Invoke(this);
        }

        protected void DeleteThisNode()
        {
            OnUINodeDeleted?.Invoke(this);
        }



        //---------------------------------
        // Abstract methods
        //---------------------------------

        public abstract void OnDraw();
        public abstract void DrawConnections();
        protected abstract void ProcessContextMenu();
        protected abstract void OnSetSelected(bool selected);
    }



    //--------------------------------------
    // Speech Node
    //--------------------------------------

    public class UISpeechNode : UINode
    {
        protected const float SPRITE_SZ = 40;
        protected const int NAME_HEIGHT = 12;

        // Events
        public delegate void CreateOptionEvent(UISpeechNode node);
        public static CreateOptionEvent OnCreateOption;

        // Static properties
        public static int Width { get { return 200; } }
        public static int Height { get { return 80; } }

        // Properties
        public EditableSpeechNode SpeechNode { get { return Info as EditableSpeechNode; } }
        public override Color DefaultColor { get { return DialogueEditorUtil.Colour(189, 0, 0); } }
        public override Color SelectedColor { get { return DialogueEditorUtil.Colour(255, 0, 0); } }

        // Static styles
        protected static GUIStyle defaultNodeStyle;
        protected static GUIStyle selectedNodeStyle;

        protected static GUIStyle npcNameStyle;


        //---------------------------------
        // Constructor
        //---------------------------------

        public UISpeechNode(EditableConversationNode infoNode, Vector2 pos) : base(infoNode, pos)
        {
            if (defaultNodeStyle == null || defaultNodeStyle.normal.background == null)
            {
                defaultNodeStyle = new GUIStyle();
                defaultNodeStyle.normal.background = DialogueEditorUtil.MakeTextureForNode(Width, Height, DefaultColor);
            }
            if (selectedNodeStyle == null || selectedNodeStyle.normal.background == null)
            {
                selectedNodeStyle = new GUIStyle();
                selectedNodeStyle.normal.background = DialogueEditorUtil.MakeTextureForNode(Width, Height, SelectedColor);
            }
            if (npcNameStyle == null)
            {
                npcNameStyle = new GUIStyle();
                npcNameStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1);
                npcNameStyle.wordWrap = true;
                npcNameStyle.stretchHeight = false;
                npcNameStyle.alignment = TextAnchor.MiddleCenter;
                npcNameStyle.clipping = TextClipping.Clip;
            }

            currentBoxStyle = defaultNodeStyle;

            CreateRect(pos, viewOffset, Width, Height);
        }



        //---------------------------------
        // Drawing
        //---------------------------------

        public override void OnDraw()
        {
            if (DialogueEditorWindow.ConversationRoot == SpeechNode)
                DrawTitle(isSelected ? "[Root]Speech node (selected)." : "[Root] Speech node.");
            else
                DrawTitle(isSelected ? "Speech node (selected)." : "Speech node.");

            // Name
            const int NAME_PADDING = 1;
            Rect name = new Rect(rect.x + TEXT_BORDER * 0.5f, rect.y + NAME_PADDING + TITLE_HEIGHT, rect.width - TEXT_BORDER * 0.5f, NAME_HEIGHT);
            GUI.Box(name, SpeechNode.Name, npcNameStyle);

            // Icon
            Rect icon = new Rect(rect.x + TEXT_BORDER * 0.5f, rect.y + TITLE_HEIGHT + TITLE_GAP + NAME_HEIGHT, SPRITE_SZ, SPRITE_SZ);
            if (SpeechNode.Icon != null)
                GUI.DrawTexture(icon, SpeechNode.Icon.texture, ScaleMode.ScaleToFit);

            // Text
            DrawInternalText(SpeechNode.Text, SPRITE_SZ + 5, NAME_HEIGHT + NAME_PADDING);
        }

        public override void DrawConnections()
        {
            if (SpeechNode.Options != null && SpeechNode.Options.Count > 0)
            {
                Vector2 start, end;
                for (int i = 0; i < SpeechNode.Options.Count; i++)
                {
                    DialogueEditorUtil.GetConnectionDrawInfo(rect, SpeechNode.Options[i], out start, out end);
                    end += viewOffset;

                    Vector2 toStart = (start - end).normalized;
                    Vector2 toEnd = (end - start).normalized;
                    Handles.DrawBezier(start, end, start + toStart, end + toEnd, DefaultColor, null, LINE_WIDTH);

                    Vector2 intersection;
                    Vector2 boxPos = new Vector2(SpeechNode.Options[i].EditorInfo.xPos + viewOffset.x, SpeechNode.Options[i].EditorInfo.yPos + viewOffset.y);
                    if (DialogueEditorUtil.DoesLineIntersectWithBox(start, end, boxPos, true, out intersection))
                    {
                        DialogueEditorUtil.DrawArrowTip(intersection, toEnd, DefaultColor);
                    }
                }
            }
            else if (SpeechNode.Speech != null)
            {
                Vector2 start, end;
                DialogueEditorUtil.GetConnectionDrawInfo(rect, SpeechNode.Speech, out start, out end);
                end += viewOffset;
                
                Vector2 toStart = (start - end).normalized;
                Vector2 toEnd = (end - start).normalized;
                Handles.DrawBezier(start, end, start + toStart, end + toEnd, DefaultColor, null, LINE_WIDTH);

                Vector2 intersection;
                Vector2 boxPos = new Vector2(SpeechNode.Speech.EditorInfo.xPos + viewOffset.x, SpeechNode.Speech.EditorInfo.yPos + viewOffset.y);
                if (DialogueEditorUtil.DoesLineIntersectWithBox(start, end, boxPos, false, out intersection))
                {
                    DialogueEditorUtil.DrawArrowTip(intersection, toEnd, DefaultColor);
                }
            }

        }




        //---------------------------------
        // Interactions
        //---------------------------------

        protected override void OnSetSelected(bool selected)
        {
            if (selected)
                currentBoxStyle = selectedNodeStyle;
            else
                currentBoxStyle = defaultNodeStyle;
        }




        //---------------------------------
        // Right clicked
        //---------------------------------

        protected override void ProcessContextMenu()
        {
            GenericMenu rightClickMenu = new GenericMenu();
            rightClickMenu.AddItem(new GUIContent("Create Option"), false, CreateOption);
            rightClickMenu.AddItem(new GUIContent("Create Speech"), false, CreateSpeech);
            rightClickMenu.AddItem(new GUIContent("Connect"), false, ConnectToNode);
            rightClickMenu.AddItem(new GUIContent("Delete"), false, DeleteThisNode);
            rightClickMenu.ShowAsContext();
        }

        private void CreateOption()
        {
            OnCreateOption?.Invoke(this);
        }
    }




    //--------------------------------------
    // OptionNode
    //--------------------------------------

    public class UIOptionNode : UINode
    {
        // Static properties
        public static int Width { get { return 200; } }
        public static int Height { get { return 50; } }

        // Properties
        public EditableOptionNode OptionNode { get { return Info as EditableOptionNode; } }
        public override Color DefaultColor { get { return DialogueEditorUtil.Colour(0, 158, 118); } }
        public override Color SelectedColor { get { return DialogueEditorUtil.Colour(0, 201, 150); } }

        // Static styles
        protected static GUIStyle defaultNodeStyle;
        protected static GUIStyle selectedNodeStyle;


        //---------------------------------
        // Constructor 
        //---------------------------------

        public UIOptionNode(EditableConversationNode infoNode, Vector2 pos) : base(infoNode, pos)
        {
            if (defaultNodeStyle == null || defaultNodeStyle.normal.background == null)
            {
                defaultNodeStyle = new GUIStyle();
                defaultNodeStyle.normal.background = DialogueEditorUtil.MakeTextureForNode(Width, Height, DefaultColor);
            }
            if (selectedNodeStyle == null || selectedNodeStyle.normal.background == null)
            {
                selectedNodeStyle = new GUIStyle();
                selectedNodeStyle.normal.background = DialogueEditorUtil.MakeTextureForNode(Width, Height, SelectedColor);
            }

            currentBoxStyle = defaultNodeStyle;

            CreateRect(pos, viewOffset, Width, Height);
        }




        //---------------------------------
        // Drawing
        //---------------------------------

        public override void OnDraw()
        {
            DrawTitle( isSelected ? "Option node (selected)." : "Option node.");
            DrawInternalText(OptionNode.Text);
        }

        public override void DrawConnections()
        {
            if (OptionNode.Speech != null)
            {
                Vector2 start, end;
                DialogueEditorUtil.GetConnectionDrawInfo(rect, OptionNode.Speech, out start, out end);

                end += viewOffset;

                Vector2 toStart = (start - end).normalized;
                Vector2 toEnd = (end - start).normalized;
                Handles.DrawBezier(start, end, start + toStart, end + toEnd, DefaultColor, null, LINE_WIDTH);

                Vector2 intersection;
                Vector2 boxPos = new Vector2(OptionNode.Speech.EditorInfo.xPos + viewOffset.x, OptionNode.Speech.EditorInfo.yPos + viewOffset.y);
                if (DialogueEditorUtil.DoesLineIntersectWithBox(start, end, boxPos, false, out intersection))
                {
                    DialogueEditorUtil.DrawArrowTip(intersection, toEnd, DefaultColor);
                }
            }
        }




        //---------------------------------
        // Interactions
        //---------------------------------

        protected override void OnSetSelected(bool selected)
        {
            if (selected)
                currentBoxStyle = selectedNodeStyle;
            else
                currentBoxStyle = defaultNodeStyle;
        }




        //---------------------------------
        // Right clicked
        //---------------------------------

        protected override void ProcessContextMenu()
        {
            GenericMenu rightClickMenu = new GenericMenu();
            rightClickMenu.AddItem(new GUIContent("Create Speech"), false, CreateSpeech);
            rightClickMenu.AddItem(new GUIContent("Connect"), false, ConnectToNode);
            rightClickMenu.AddItem(new GUIContent("Delete"), false, DeleteThisNode);
            rightClickMenu.ShowAsContext();
        }
    }
}