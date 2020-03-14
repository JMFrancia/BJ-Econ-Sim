using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//Place an empty object with this script in the scene
public class AttackerBeeController : MonoBehaviour
{
    public static AttackerBeeController Instance;

    [Tooltip("Attacker bee prefab to generate")]
    [SerializeField] GameObject attackerBeePrefab;

    [Tooltip("Speed of attacker movmeent")]
    public float AttackerSpeed = .5f;
    [Tooltip("Frequency of attacker generation")]
    public float AttackerGenFrequency = .33f;   //Frequency of attacker generation1

    //Dictionary for fetching each individual battle's coroutine
    Dictionary<Transform, Coroutine> attacks = new Dictionary<Transform, Coroutine>();
    //Dictionary for fetching all bees in a given battle
    Dictionary<Transform, List<AttackerBee>> attackers = new Dictionary<Transform, List<AttackerBee>>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }
    }

    //Call this on the transform of the enemy bee once guard engages them. Or if that doesn't work,
    //Call this on the transform of the battle cloud that is created when guard engages them
    public void StartAttack(Transform enemy) {
        attackers[enemy] = new List<AttackerBee>();
        attacks[enemy] = StartCoroutine(GenerateAttackers(enemy));
    }

    //Ends attack on given transform. When attack ends, attackers automatically
    //join another attack, or if none else occuring, fly off screen
    public void EndAttack(Transform enemy) {
        StopCoroutine(attacks[enemy]);
        attacks.Remove(enemy);
        foreach (AttackerBee attacker in attackers[enemy]) {
            //If other attacks going on, pick one randomly and go into it. Otherwise fly offscreen
            if (attacks.Count > 0) {
                int randomTargetIndex = Random.Range(0, attacks.Keys.Count);
                Transform randomTarget = attacks.Keys.ToArray()[randomTargetIndex];
                attacker.SetTarget(randomTarget.transform.position, AttackerSpeed);
                attackers[enemy].Add(attacker);
            } else {
                CancelAttack(attacker);
            }
        }
        attackers.Remove(enemy);
    }

    //Ends all attacks, useful for when invasion ends
    public void EndAllAttacks() {
        attacks.Keys.ToList().ForEach(EndAttack);
    }

    //Increase speed and frequency of all attackers
    public void GoTurbo()
    {
        StartCoroutine(GoTurbo(3f, 5f));
    }

    void CancelAttack(AttackerBee attacker) {
        attacker.SetTarget(GetRandomPosOffScreen(), AttackerSpeed);
    }

    IEnumerator GoTurbo(float multiplier, float duration) {
        AttackerSpeed *= multiplier;
        AttackerGenFrequency *= multiplier;
        yield return new WaitForSeconds(duration);
        AttackerSpeed /= multiplier;
        AttackerGenFrequency /= multiplier;
    }

    //Repeatedly generates attacker bees toward target until stopped via EndAttack()
    IEnumerator GenerateAttackers(Transform target) {
        while (true) {
            GameObject newAttackerGO = Instantiate(attackerBeePrefab, GetRandomPosOffScreen(), Quaternion.identity);
            AttackerBee newAttacker = newAttackerGO.GetComponent<AttackerBee>();
            newAttacker.SetTarget(target.position, AttackerSpeed);
            attackers[target].Add(newAttacker);
            yield return new WaitForSeconds(AttackerGenFrequency); ;
        }
    }

    //Helper function to get random position just off-screen
    Vector2 GetRandomPosOffScreen()
    {
        //We want to get position slightly off-screen, not exactly on the edge of it,
        float offScreenAdjustment = .1f;
        List<Vector2> directions = new List<Vector2>();

        //Generate all possible position just off-screen in 8 cardinal directions and pick one randomly
        float[] edgePossibilities = new float[] {
            0 - offScreenAdjustment,
            0,
            1 + offScreenAdjustment
        };
        for (int n = 0; n < edgePossibilities.Length; n++) {
            for (int k = 0; k < edgePossibilities.Length; k++) {
                if (Mathf.Approximately(edgePossibilities[n], 0) && Mathf.Approximately(edgePossibilities[k], 0))
                    continue;
                directions.Add(new Vector2(edgePossibilities[n], edgePossibilities[k]));
            }
        }
        Vector2 randomEdge = directions[Random.Range(0, directions.Count)];
        return (Vector2)Camera.main.ViewportToWorldPoint(randomEdge);
    }
}
