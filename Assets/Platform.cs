using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Platform : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Vector3 startPos;
    public GameObject platform;
    public float maxDrop = 10f;
    private string sceneName;
    public bool weighted, onPlat;
    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        startPos = transform.position;
        _rigidbody.isKinematic = true;
        var platRenderer = this.gameObject.GetComponent<Renderer>();
        platRenderer.material.SetColor("_Color", Color.white);
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
    }
    // Update is called once per frame
    void Update()
    {
        if(!weighted)
        {
            if (transform.position.y < (startPos.y - maxDrop))
            {//After the platform has dropped a certain distance it will be reset
                Instantiate(platform, startPos, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
        else
        {
            if (onPlat)
            {//Drop the platform when in contact with the player
                Vector2 dropPosition = new Vector2(startPos.x, (startPos.y - maxDrop));
                transform.position = Vector2.Lerp(transform.position, dropPosition, Time.deltaTime * 0.7f);
            }
            else if(transform.position.y < startPos.y && !onPlat)
            {
                transform.position = Vector2.Lerp(transform.position, startPos, Time.deltaTime * 1.3f);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            onPlat = true;
            if(sceneName == "ZeusRoom")
            {//Change the colour depending on the room
                var platRenderer = this.gameObject.GetComponent<Renderer>();
                platRenderer.material.SetColor("_Color", Color.grey);
            }
            else if(sceneName == "HadesRoom")
            {
                var platRenderer = this.gameObject.GetComponent<Renderer>();
                platRenderer.material.SetColor("_Color", Color.red);
            }
            if(!weighted)
            {
                _rigidbody.drag = 10;
                Invoke("DropPlatform", 0.5f);
            }
            else
            {

            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        _rigidbody.drag = 0;
        onPlat = false;
    }
    void DropPlatform()
    {//Change the physics when the player is on the platform
        _rigidbody.isKinematic = false;
    }
}
