using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UniTaskTutorial.BaseUsing.Scripts
{
    public class UniTaskBaseTest : MonoBehaviour
    {
        public Button LoadTextButton;
        public Text TargetText;

        public Button LoadSceneButton;
        public Slider LoadSceneSlider;
        public Text ProgressText;

        public Button WebRequestButton;
        public Image DownloadImage;

        private void Start()
        {
            LoadTextButton.onClick.AddListener(OnClickLoadText);
            LoadSceneButton.onClick.AddListener(OnClickLoadScene);
            WebRequestButton.onClick.AddListener(OnClickWebRequest);
        }

        private async void OnClickWebRequest()
        {
            var webRequest =
                UnityWebRequestTexture.GetTexture(
                    "https://s1.hdslb.com/bfs/static/jinkela/video/asserts/33-coin-ani.png");
            var result = (await webRequest.SendWebRequest());
            var texture = ((DownloadHandlerTexture) result.downloadHandler).texture;
            int totalSpriteCount = 24;
            int perSpriteWidth = texture.width / totalSpriteCount;
            Sprite[] sprites = new Sprite[totalSpriteCount];
            for (int i = 0; i < totalSpriteCount; i++)
            {
                sprites[i] = Sprite.Create(texture,
                    new Rect(new Vector2(perSpriteWidth * i, 0), new Vector2(perSpriteWidth, texture.height)),
                    new Vector2(0.5f, 0.5f));
            }

            float perFrameTime = 0.1f;
            while (true)
            {
                for (int i = 0; i < totalSpriteCount; i++)
                {
                    await Cysharp.Threading.Tasks.UniTask.Delay(TimeSpan.FromSeconds(perFrameTime));
                    var sprite = sprites[i];
                    DownloadImage.sprite = sprite;
                }
            }
        }

        private async void OnClickLoadScene()
        {
            await SceneManager.LoadSceneAsync("Box/UniTask/UniTaskTutorial/BaseUsing/Scenes/TargetLoadScene").ToUniTask(
                (Progress.Create<float>(
                    (p) =>
                    {
                        LoadSceneSlider.value = p;
                        if (ProgressText != null)
                        {
                            ProgressText.text = $"读取进度{p * 100:F2}%";
                        }
                    })));
        }

        private async void OnClickLoadText()
        {
            var loadOperation = Resources.LoadAsync<TextAsset>("test");
            var text = await loadOperation;

            TargetText.text = ((TextAsset) text).text;

            // UniTaskAsyncSample_Base asyncUnitaskLoader = new UniTaskAsyncSample_Base();
            // TargetText.text = ((TextAsset) (await asyncUnitaskLoader.LoadAsync<TextAsset>("test"))).text;
        }
    }
}