using Game.Weapons;
using Game.Weapons.Guns;
using UnityEngine;

public abstract class Projectile : MonoBehaviour, IPoolable {

    public PoolBehaviour PoolParent { get; set; }
    
    protected Gun Gun;
    protected Muzzle Muzzle;
    public virtual void InitProjectile(Gun gun, Muzzle muzzle) {
        Gun = gun;
        Muzzle = muzzle;
        PoolParent = muzzle;
    }
    
    public abstract void UpdateProjectile(float deltaTime);
    
    public abstract void Pool();

    public abstract void Unpool();

    public abstract void Clean();
}
