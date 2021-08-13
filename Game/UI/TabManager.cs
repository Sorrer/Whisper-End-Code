using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {
    public class TabManager : MonoBehaviour
    {
        [Serializable]
        public struct Tab {
            public TabObject TabPanel;
            public Button button;
        }

        public List<Tab> Tabs = new List<Tab>();
        
        
        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < Tabs.Count; i++) {
                var tab = Tabs[i];

                var index = i;
                tab.button.onClick.AddListener(() => {
                    OnClicked(index);
                });
                
                tab.TabPanel.Deactive();
            }
        }

        public void OnClicked(int index) {

            for (int i = 0; i < Tabs.Count; i++) {
                if (i == index) {
                    Tabs[i].TabPanel.Activate();
                } else {
                    Tabs[i].TabPanel.Deactive();
                }
            }
            
        }
    }
}
