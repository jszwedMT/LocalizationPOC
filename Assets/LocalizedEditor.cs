using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Utilities.Localization
{
    public class LocalizedEditor : EditorWindowBuilder
    {
        public class LoadedLanguages
        {
            public SortedDictionary<string, string>[] languages;
        }

        public class KeyGroup
        {
            public string groupName;
            public Enum[] keys;
        }

        public class Keys
        {
            public KeyGroup[] keyGroups;
        }

        public const string TITLE = "Localization";

        public const int COLUMN_WIDTH = 300;
        public const int ROW_HEIGHT = 20;

        public const int KEYS_COLUMN = 150;

        public const int MARGIN_X = 10;
        public const int MARGIN_Y = 10;

        float scrollX = 0;
        float scrollY = 0;

        string search = "";

        public LoadedLanguages loadedLanguages;
        public Keys keys;
        public Keys filteredKeys = new Keys();

        [MenuItem("Window/Localization Editor")]
        public static void ShowWindow()
        {
            Localized.Instance.ValidateAndCreateLanguages();
            var editorWindow = EditorWindow.GetWindow<LocalizedEditor>(TITLE);
            editorWindow.LoadLanguages();
            editorWindow.GenerateKeys(true);
        }
        
        private void OnGUI()
        {
            LoadLanguages();
            GenerateKeys(false);

            SearchKeys((int)scrollX, 10 - (int)scrollY);
            
            float width = GenerateLocalizationHeaders((int)scrollX, 40 - (int)scrollY);
            float height = GenerateVisibleKeys((int)width, (int)scrollX, 40 + ROW_HEIGHT - (int)scrollY);
            
            if (position.width < width)
            {
                scrollX = GUI.HorizontalScrollbar(new Rect(0, 0, position.width - 15, 30), scrollX, position.width, 0, width);
            }
            else
            {
                scrollX = 0;
            }

            if (position.height < height)
            {
                scrollY = GUI.VerticalScrollbar(new Rect(position.width - 15, 30, 30, position.height - 30), scrollY, position.height, 0, height);
            }
            else
            {
                scrollY = 0;
            }

            CheckDiry();
        }

        public void CheckDiry()
        {
            if (isDirty)
            {
                titleContent.text = TITLE + "*";
            }

            var currentEvent = Event.current;
            if (isDirty && currentEvent != null && (currentEvent.type == EventType.KeyUp || currentEvent.type == EventType.KeyDown) && currentEvent.control && currentEvent.keyCode == KeyCode.S)
            {
                isDirty = false;
                titleContent.text = TITLE;
                GUI.changed = true;

                var languages = Utilities.GetEnums<Localized.Languages>().ToArray();
                for (int i = 0; i < languages.Length; i++)
                {
                    Localized.SaveLanguage(languages[i], loadedLanguages.languages[i]);
                }

                Debug.Log("saved localizations");
            }
        }

        public void GenerateKeys(bool force)
        {
            if (keys != null && !force)
            {
                return;
            }

            List<KeyGroup> keyGroups = new List<KeyGroup>();
            foreach (var enumType in typeof(Localized).GetNestedTypes())
            {
                if (!enumType.IsEnum)
                {
                    continue;
                }

                List<Enum> keys = new List<Enum>();
                foreach (var enumVal in Utilities.GetEnums(enumType))
                {
                    keys.Add(enumVal);
                }

                keyGroups.Add(new KeyGroup()
                {
                    groupName = enumType.Name,
                    keys = keys.ToArray()
                });
            }

            keys = new Keys();
            keys.keyGroups = keyGroups.OrderBy(x => x.groupName).ToArray();
        }

        public int GenerateLocalizationHeaders(int horizontalOffset = 0, int verticalOffset = 0)
        {
            int headerIndex = 0;
            int headerWidth = MARGIN_X;

            foreach (var languages in  Utilities.GetEnums<Localized.Languages>())
            {
                var displayAttribute = languages.GetAttribute<DisplayNameAttribute>();
                string displayName = displayAttribute == null ? languages.ToString() : displayAttribute.Name;
                
                Header(displayName, headerWidth - horizontalOffset, MARGIN_Y + verticalOffset);
                if (headerIndex == 0)
                {
                    headerWidth += KEYS_COLUMN;
                }
                else
                {
                    VerticalLine(headerWidth - MARGIN_X - horizontalOffset, MARGIN_Y + verticalOffset, ROW_HEIGHT - 2);
                    headerWidth += COLUMN_WIDTH;
                }
                headerIndex++;
            }

            HorizontalLine(MARGIN_X - horizontalOffset, MARGIN_Y + ROW_HEIGHT + verticalOffset, headerWidth, 3);
            return headerWidth;
        }

        public int GenerateVisibleKeys(int width, int horizontalOffset = 0, int verticalOffset = 0)
        {
            int offset = 0;
            foreach (var group in filteredKeys.keyGroups)
            {
                int topOfSection = offset;
                
                offset += ROW_HEIGHT;
                Header(group.groupName, MARGIN_X - horizontalOffset, verticalOffset + offset + MARGIN_Y);
                offset += ROW_HEIGHT;

                HorizontalLine(MARGIN_X - horizontalOffset, verticalOffset + offset + MARGIN_Y, width, 3);
                
                foreach (var key in group.keys)
                {
                    var attribute = key.GetAttribute<DescriptionAttribute>();
                    if (attribute == null)
                    {
                        Label(key.ToString(), MARGIN_X - horizontalOffset, verticalOffset + offset + MARGIN_Y);
                    }
                    else
                    {
                        Label(key.ToString(), MARGIN_X - horizontalOffset, verticalOffset + offset + MARGIN_Y, tooltip: attribute.Description);
                    }


                    for (int i = 1; i < loadedLanguages.languages.Length; i++)
                    {
                        loadedLanguages.languages[i][key.ToString()] = TextInput(loadedLanguages.languages[i][key.ToString()]
                            , -horizontalOffset + KEYS_COLUMN + COLUMN_WIDTH * (i - 1)
                            , verticalOffset + offset + MARGIN_Y
                            , COLUMN_WIDTH
                            , ROW_HEIGHT);
                    }

                    offset += ROW_HEIGHT;
                    HorizontalLine(MARGIN_X - horizontalOffset, verticalOffset + offset + MARGIN_Y, width, 1);
                }

                int columnMargin = MARGIN_X + KEYS_COLUMN;
                for (int i = 1; i < loadedLanguages.languages.Length; i++)
                {
                    VerticalLine(columnMargin - MARGIN_X - horizontalOffset, topOfSection + verticalOffset + MARGIN_Y + ROW_HEIGHT, offset - topOfSection - ROW_HEIGHT, 1);
                    columnMargin += COLUMN_WIDTH;
                }
            }

            return offset;
        }

        public void SearchKeys(int horizontalOffset = 0, int verticalOffset = 0)
        {
            if (string.IsNullOrEmpty(search))
            {
                filteredKeys.keyGroups = keys.keyGroups;
            }

            Label("Search:", MARGIN_X - horizontalOffset, MARGIN_Y + verticalOffset);
            string newSearch = TextInput(search, 100 + MARGIN_X - horizontalOffset, MARGIN_Y + verticalOffset, isDirtiable: false);
            
            if (!newSearch.Equals(search))
            {
                search = newSearch;

                List<KeyGroup> searched = new List<KeyGroup>();
                if (!string.IsNullOrEmpty(search))
                {
                    foreach (var keygroup in keys.keyGroups)
                    {
                        if (keygroup.groupName.Contains(search, StringComparison.OrdinalIgnoreCase))
                        {
                            KeyGroup searchgroup = new KeyGroup();
                            searchgroup.groupName = keygroup.groupName;
                            searchgroup.keys = keygroup.keys;
                            searched.Add(searchgroup);
                        }
                        else
                        {
                            var keys = keygroup.keys.Where(x => x.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)).ToArray();
                            if (keys.Length > 0)
                            {
                                KeyGroup searchgroup = new KeyGroup();
                                searchgroup.groupName = keygroup.groupName;
                                searchgroup.keys = keys;
                                searched.Add(searchgroup);
                            }
                        }
                    }
                }

                filteredKeys.keyGroups = searched.ToArray();
            }
        }

        public void LoadLanguages()
        {
            if (loadedLanguages == null)
            {
                loadedLanguages = new LoadedLanguages();
            }

            if (loadedLanguages.languages == null)
            {
                var languages = Utilities.GetEnums<Localization.Localized.Languages>().ToArray();
                int languageCount = languages.Length;

                loadedLanguages.languages = new SortedDictionary<string, string>[languageCount];
                for (int i = 0; i < languageCount; i++)
                {
                    loadedLanguages.languages[i] = Localized.Instance.LoadLanguage(languages[i]);
                }
            }
        }
    }
}
