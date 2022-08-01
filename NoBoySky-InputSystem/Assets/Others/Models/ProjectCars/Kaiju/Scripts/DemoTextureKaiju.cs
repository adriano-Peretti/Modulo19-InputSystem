using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class DemoTextureKaiju : MonoBehaviour
{
    public Renderer bodyRenderer;
    public Renderer[] wheelRenderers;

    public GUIStyle _text;

    public Material _body;
    public Material _wheel;

    public Texture[] _texture;
    public Texture[] _textureOther;

    public int _numTexture;

    private void Start()
    {
        _body = new Material(_body);
        _body.name = _body.name + " (Instanced)";
        _wheel = new Material(_wheel);
        _wheel.name = _wheel.name + " (Instanced)";

        bodyRenderer.material = _body;

        foreach(Renderer rend in wheelRenderers)
        {
            rend.material = _wheel;
        }
    }

    void OnNext(InputValue value)
    {
        AdvanceTexture();
    }

    void OnPrevious(InputValue value)
    {
        ReturnTexture();
    }

    void ReturnTexture()
    {
        if (_numTexture > 0)
        {
            _numTexture--;
        }

        _body.mainTexture = _texture[_numTexture];
        _wheel.mainTexture = _textureOther[_numTexture];
    }

    void AdvanceTexture()
    {
        if (_numTexture < _texture.Length - 1)
        {
            _numTexture++;
        }

        _body.mainTexture = _texture[_numTexture];
        _wheel.mainTexture = _textureOther[_numTexture];
    }

    void OnGUI()
    {
        GUI.Box(new Rect(20, 20, Screen.width, 100), "For view next texture press 'D' or 'A' for view back texture\n View texture: " + (_numTexture+1) + "/" + _texture.Length, _text);
    }
}
