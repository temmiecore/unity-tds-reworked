using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Transform), typeof(AIPath), typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Health))]
public class BTreeController : Tree
{
    [HideInInspector] public Transform npcTransform;
    [HideInInspector] public AIPath pathfinder;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody2D rigidbody;
    [HideInInspector] public CapsuleCollider2D capsuleCollider;

    public NPCWeaponController enemyWeapon;
    public Transform enemyHand;
    public BehaviourTreeType behaviourTreeType;
    public bool isFriendly;

    [Range(0,10f)]
    public float alertRadius;
    [Range(0, 10f)]
    public float ignoreRadius;
    [Range(0, 10f)]
    public float attackRadius;
    public float attackCooldown;

    [Header("For NPC/Enemies who follow something")]
    public Transform followTarget;
    public float followRadius;

    [Header("Target")]
    public Transform target;
    public Dictionary<Transform, float> threatList;
    public List<BTreeController> attackersList;

    [Header("Loot table")]
    public List<GameObject> itemPrefabs;
    [Range(0, 1f)]
    public List<float> itemWeights;

    public int droppedXP;

    protected override void Start()
    {
        npcTransform = GetComponent<Transform>();
        pathfinder = GetComponent<AIPath>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        target = null;
        threatList = new Dictionary<Transform, float>();

        base.Start();
    }

    protected override Node SetupTree()
    {
        return GameManager.Instance.behaviourTreeManager.SetupTree(behaviourTreeType, this);
    }

    public void DropLoot()
    {
        if (itemPrefabs.Count == 0 || itemWeights.Count != itemPrefabs.Count)
            return;

        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            if (Random.Range(0,1f) < itemWeights[i])
                Instantiate(itemPrefabs[i], transform.position, transform.rotation);
        }
    }

    public void DropXP()
    {
        GameManager.Instance.PlayerAddXP(droppedXP);
    }

    public void AddThreat(Transform attacker)
    {
        if (attacker.TryGetComponent(out BTreeController npcComponent))
        {
            threatList[attacker] += 1;
        }
    }

    public void OnDeath(Transform attacker)
    {
        if (attacker.TryGetComponent(out Player playerComponent))
        {
            DropLoot();
            DropXP();
        }

        if (attacker.TryGetComponent(out BTreeController npcComponent))
        {
            foreach (BTreeController attack in attackersList)
            {
                attack.target = null;
                attack.threatList.Remove(transform);
                attack.attackersList.Remove(this);
            }
        }

        Destroy(gameObject);
    }
}

public enum BehaviourTreeType
{
    WanderingCircle,
    Wandering,
    Patrol,
}