namespace UnityEngine.Networking
{
	[AddComponentMenu("Network/NetworkManagerHUD")]
	[RequireComponent(typeof(NetworkManager))]
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class NetworkManagerHUD : MonoBehaviour
	{
		public NetworkManager manager;
		[SerializeField] public bool showGUI = true;
		[SerializeField] public int offsetX;
		[SerializeField] public int offsetY;
        [SerializeField] private GUIStyle _buttonStyle;
        [SerializeField] public GUIStyle txtStyle;
        [SerializeField] public GUIStyle labelStyle;

        // Runtime variable
        bool showServer = false;

        public GUIStyle ButtonStyle
        {
            get
            {
                return _buttonStyle;
            }

            set
            {
                _buttonStyle = value;
            }
        }

        void Awake()
		{
			manager = GetComponent<NetworkManager>();
		}

		void Update()
		{
			if (!showGUI)
				return;

			if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
			{
				if (Input.GetKeyDown(KeyCode.S))
				{
					manager.StartServer();
				}
				if (Input.GetKeyDown(KeyCode.H))
				{
					manager.StartHost();
				}
				if (Input.GetKeyDown(KeyCode.C))
				{
					manager.StartClient();
				}
			}
			if (NetworkServer.active && NetworkClient.active)
			{
				if (Input.GetKeyDown(KeyCode.X))
				{
					manager.StopHost();
				}
			}
		}

		void OnGUI()
		{
            ButtonStyle = GUI.skin.GetStyle("Button");
            ButtonStyle.fontSize = 40;

            txtStyle = GUI.skin.GetStyle("TextField");
            txtStyle.fontSize = 40;
            txtStyle.alignment = TextAnchor.MiddleLeft;
            txtStyle.contentOffset.Set(5, 0);

            labelStyle = GUI.skin.GetStyle("Label");
            labelStyle.fontSize = 26;

            
            if (!showGUI)
				return;

			int xpos = 10 + offsetX;
			int ypos = 30 + offsetY;
			int spacing = 50;
            int spacing2 = 80;

			if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
			{
                if (GUI.Button(new Rect(xpos, ypos, 400, 70), "LAN Host(H)", ButtonStyle))
				{
					manager.StartHost();
				}
				ypos += spacing2;

				if (GUI.Button(new Rect(xpos, ypos, 400, 70), "LAN Client(C)", ButtonStyle))
				{
					manager.StartClient();
				}
				manager.networkAddress = GUI.TextField(new Rect(xpos + 410, ypos, 395, 70), manager.networkAddress, txtStyle);
				ypos += spacing2;

				if (GUI.Button(new Rect(xpos, ypos, 400, 70), "LAN Server Only(S)", ButtonStyle))
				{
					manager.StartServer();
				}
				ypos += spacing2;
			}
			else
			{
                int yOld = ypos;
                int xOld = xpos;
				if (NetworkServer.active)
				{
					GUI.Label(new Rect(xpos, ypos, 260, 40), "Server: port=" + manager.networkPort, labelStyle);
					ypos += spacing;
				}
				if (NetworkClient.active)
				{
                    if (yOld != ypos)
                    {
                        ypos = yOld;
                        xpos += 260;
                    }
					GUI.Label(new Rect(xpos, ypos, 500, 40), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort, labelStyle);
					ypos += spacing;
                    xpos = xOld;
				}
			}

			if (NetworkClient.active && !ClientScene.ready)
			{
				if (GUI.Button(new Rect(xpos, ypos, 220, 20), "Client Ready", ButtonStyle))
				{
					ClientScene.Ready(manager.client.connection);
				
					if (ClientScene.localPlayers.Count == 0)
					{
						ClientScene.AddPlayer(0);
					}
				}
				ypos += spacing;
			}

			if (NetworkServer.active || NetworkClient.active)
			{
				if (GUI.Button(new Rect(xpos, ypos, 200, 50), "Quit (X)", ButtonStyle))
				{
					manager.StopHost();
				}
				ypos += spacing;
			}

			//if (!NetworkServer.active && !NetworkClient.active)
			//{
			//	ypos += 10;

			//	if (manager.matchMaker == null)
			//	{
			//		if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Enable Match Maker (M)"))
			//		{
			//			manager.StartMatchMaker();
			//		}
			//		ypos += spacing;
			//	}
			//	else
			//	{
			//		if (manager.matchInfo == null)
			//		{
			//			if (manager.matches == null)
			//			{
			//				if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Create Internet Match"))
			//				{
			//					manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", manager.OnMatchCreate);
			//				}
			//				ypos += spacing;

			//				GUI.Label(new Rect(xpos, ypos, 100, 20), "Room Name:");
			//				manager.matchName = GUI.TextField(new Rect(xpos+100, ypos, 100, 20), manager.matchName);
			//				ypos += spacing;

			//				ypos += 10;

			//				if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Find Internet Match"))
			//				{
			//					manager.matchMaker.ListMatches(0,20, "", manager.OnMatchList);
			//				}
			//				ypos += spacing;
			//			}
			//			else
			//			{
			//				foreach (var match in manager.matches)
			//				{
			//					if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Join Match:" + match.name))
			//					{
			//						manager.matchName = match.name;
			//						manager.matchSize = (uint)match.currentSize;
			//						manager.matchMaker.JoinMatch(match.networkId, "", manager.OnMatchJoined);
			//					}
			//					ypos += spacing;
			//				}
			//			}
			//		}

			//		if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Change MM server"))
			//		{
			//			showServer = !showServer;
			//		}
			//		if (showServer)
			//		{
			//			ypos += spacing;
			//			if (GUI.Button(new Rect(xpos, ypos, 100, 20), "Local"))
			//			{
			//				manager.SetMatchHost("localhost", 1337, false);
			//				showServer = false;
			//			}
			//			ypos += spacing;
			//			if (GUI.Button(new Rect(xpos, ypos, 100, 20), "Internet"))
			//			{
			//				manager.SetMatchHost("mm.unet.unity3d.com", 443, true);
			//				showServer = false;
			//			}
			//			ypos += spacing;
			//			if (GUI.Button(new Rect(xpos, ypos, 100, 20), "Staging"))
			//			{
			//				manager.SetMatchHost("staging-mm.unet.unity3d.com", 443, true);
			//				showServer = false;
			//			}
			//		}

			//		ypos += spacing;

			//		GUI.Label(new Rect(xpos, ypos, 300, 20), "MM Uri: " + manager.matchMaker.baseUri);
			//		ypos += spacing;

			//		if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Disable Match Maker"))
			//		{
			//			manager.StopMatchMaker();
			//		}
			//		ypos += spacing;
			//	}
			//}
		}
	}
};
