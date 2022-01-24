using System;
using NativeExtensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

// Should be placed at the other end of the table
public class PingPongBallLauncher : GMonoBehaviour
{
    [SerializeField] TwistFollower playerPaddleTwistFollower;

    [SerializeField] private GameObject pBall;
    [SerializeField] private float maxPossibleY = 5;
    [SerializeField] private float maxPossibleX;
    [SerializeField] private float minPossibleX;
    [SerializeField] private float maxPossibleBouncePointZ;
    [SerializeField] private float minPossibleBouncePointZ;


    // Launch a ball
    private void Awake()
    {
        Launch();
    }

    private void Launch()
    {
        // Start with a target in the range that the player can hit, randomly choose some parameters
        // and chart a path which will cause the ball to hit that target after a bounce
        var target = playerPaddleTwistFollower.RandomPointOnArc();
        var traversalTime = SeriesSessionManager.sPerMicroGame * .25f;

        float totalBallDisplacement = playerPaddleTwistFollower.transform.position.z - transform.position.z ;
        float zVelocity = totalBallDisplacement / traversalTime;

        var startX = Random.Range(minPossibleX, maxPossibleX);
        var startY = Random.Range(0f, maxPossibleY);
        var startZ = transform.position.z;

        var xVelocity = (target.x - startX) / traversalTime;

        var bouncePointZ = Random.Range(minPossibleBouncePointZ, maxPossibleBouncePointZ);
        var timeUntilBounce = (totalBallDisplacement + bouncePointZ) / totalBallDisplacement * traversalTime;
        var bouncePointX = timeUntilBounce * xVelocity;

        var timeAfterBounce = traversalTime - timeUntilBounce;

        // From distance formula
        // 1/2 timeAfterBounce^2 * g + v0 * timeAfterBounce = bouncePointZ
        var yVelocityAtBounce = (bouncePointZ - .5f * Mathf.Pow(timeAfterBounce, 2) * Physics.gravity.magnitude) / timeAfterBounce;

        // initialVelocity - g*t = -yVelocityAtBounce;
        var initialYVelocity = timeUntilBounce * Physics.gravity.magnitude - -yVelocityAtBounce;

        var newBallGO = Instantiate(pBall, new Vector3(startX, startY, startZ), Quaternion.identity);
        newBallGO.C<Rigidbody>().velocity = new Vector3(xVelocity, initialYVelocity, zVelocity);
    }
}