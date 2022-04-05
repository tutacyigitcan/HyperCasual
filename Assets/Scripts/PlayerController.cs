using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;


    public float limitX;
    
    public float runnigSpeed;
    public float xSpeed;
    private float _currentRunningSpeed;

    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders;

    private bool _spawningBridge;
    public GameObject bridgePiecePrefab;
    private BridgeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;

    private bool _finished;

    private float _scoreTimer = 0;

    public Animator animator;

    private float _lasttouchedX;
    private float _dropSoundTimer;

    public AudioSource cylinderAudioSource, triggerAudioSource, ItemAudioSource;
    public AudioClip getherAudioClip, dropAudioClip, coinAudioClip, buyAudioClip, equipItemAudioClip, unequipItemAudioClip;


    public List<GameObject> wearSpots;

    
    void Update()
    {
        if(LevelController.Current == null || !LevelController.Current.gameActive) 
        {
            return;
        }
        float newX = 0;
        float touchXDelta = 0;
        if(Input.touchCount > 0 )
        {
            if(Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _lasttouchedX = Input.GetTouch(0).position.x;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                touchXDelta = 5 * (Input.GetTouch(0).position.x - _lasttouchedX) / Screen.width;
                _lasttouchedX = Input.GetTouch(0).position.x;
            }
            
        }
        else if (Input.GetMouseButton(0))
        {
            touchXDelta = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);


        Vector3 newPosition = new Vector3(newX,transform.position.y,transform.position.z + _currentRunningSpeed * Time.deltaTime);
        transform.position = newPosition;


        if(_spawningBridge)
        {
            PlayDropSound();
            _creatingBridgeTimer -= Time.deltaTime;
            if(_creatingBridgeTimer < 0)
            {
                _creatingBridgeTimer = 0.01f;
                IncrementCylinderVolume(-0.01f);
                GameObject createdBrighePiece = Instantiate(bridgePiecePrefab,this.transform);
                createdBrighePiece.transform.SetParent(null);
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.StartReference.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                createdBrighePiece.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.StartReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _bridgeSpawner.StartReference.transform.position + direction * characterDistance;
                newPiecePosition.x = transform.position.x;
                createdBrighePiece.transform.position = newPiecePosition;


                if(_finished)
                {
                    _scoreTimer -= Time.deltaTime;
                    if(_scoreTimer < 0) 
                    {
                        _scoreTimer = 0.3f;
                        LevelController.Current.ChangeScore(1);
                    }
                }


            }
        }


    }


    public void ChangeSpeed(float value)
    {
        _currentRunningSpeed = value;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "AddCylinder")
        {
            cylinderAudioSource.PlayOneShot(getherAudioClip,0.1f);
            IncrementCylinderVolume(0.1f);
            Destroy(other.gameObject);
        }
        else if(other.tag == "SpawnBridge")
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag == "StopSpawnBridge")
        {
            StopSpawningBridge();
            if(_finished)
            {
                LevelController.Current.FinishGame();
            }
        }
        else if (other.tag == "Finish")
        {
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        } 
        else if (other.tag == "Coin")
        {
            triggerAudioSource.PlayOneShot(coinAudioClip, 0.1f);
            other.tag = "Untagged";
            LevelController.Current.ChangeScore(10);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(LevelController.Current.gameActive)
        {
            if (other.tag == "Tramp")
            {
                PlayDropSound();
                IncrementCylinderVolume(-Time.fixedDeltaTime);
            }
        }    
    }


    public void IncrementCylinderVolume(float value)
    {
        if(cylinders.Count == 0)
        {
            if(value > 0)
            {
                CreateCylinder(value);
            }
            else
            {
                if(_finished)
                {
                    LevelController.Current.FinishGame();
                }
                else
                {
                    Died();
                }
            }
        }
        else
        {
            cylinders[cylinders.Count - 1].IncrementCyylinderVolume(value);
        }
    }

    public void Died() 
    {
        animator.SetBool("dead", true);
        gameObject.layer = 6;
        Camera.main.transform.SetParent(null);
        LevelController.Current.GameOver();
    }

    public void CreateCylinder(float value)
    {
        RidingCylinder cretedCylinder = Instantiate(ridingCylinderPrefab, transform).GetComponent<RidingCylinder>();
        cylinders.Add(cretedCylinder);
        cretedCylinder.IncrementCyylinderVolume(value);
    }

    public void DestroyCylinder(RidingCylinder cylinder)
    {
        cylinders.Remove(cylinder);
        Destroy(cylinder.gameObject);
    }


    public void StartSpawningBridge(BridgeSpawner spawner)
    {
        _bridgeSpawner = spawner;
        _spawningBridge = true;
    }

    public void StopSpawningBridge()
    {
        _spawningBridge = false;
    }

    public void PlayDropSound()
    {
        _dropSoundTimer -= Time.deltaTime;
        if(_dropSoundTimer < 0)
        {
            _dropSoundTimer = 0.15f;
            cylinderAudioSource.PlayOneShot(dropAudioClip, 0.1f);
        }  
    }
 }
