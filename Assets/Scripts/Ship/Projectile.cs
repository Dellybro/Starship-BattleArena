using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] ParticleSystem hitEffect;
    public Health shooter;
    public int power = 10;
    public float projectileSpeed = 20f;
    public float aliveTime = 1.2f;
    public int connectionId;

    public int randomId;

    public void Setup(Health _shooter, int _cid, float _speed, int _power) {
        randomId = Random.Range(0, 99999999);
        shooter = _shooter;
        connectionId = _cid;
        projectileSpeed = _speed;
        power = _power;
    }

    void Start() {
        Destroy(gameObject, aliveTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.TryGetComponent<Health>(out Health health)){
            // Can't attack oneself
            if(shooter && shooter.gameObject) {
                if(health == shooter) return;
            }

            FindObjectOfType<SoundManager>().PlayImpactSound(transform.position, SoundManager.ImpactSoundType.Ship);

            if(health.isAi) health.DamageDealtAI(power, connectionId, randomId);
            else health.DamageDealt(power, connectionId, randomId);
            Destroy(gameObject);
        }
        else if(other.TryGetComponent<ObjectHealth>(out ObjectHealth objHealth)) {
            FindObjectOfType<SoundManager>().PlayImpactSound(transform.position, SoundManager.ImpactSoundType.Floater);
            objHealth.DamageDealt(power, connectionId);
            Destroy(gameObject);
        }
        if(other.tag == "Floaters") {
            FindObjectOfType<SoundManager>().PlayImpactSound(transform.position, SoundManager.ImpactSoundType.Planet);
            Destroy(gameObject);
        }
    }
}
