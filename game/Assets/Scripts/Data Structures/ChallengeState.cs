public class ChallengeState
{
    private PlayerState playerState;
    public PlayerState PlayerState => playerState;

    private PlayerState opponentState;
    public PlayerState OpponentState => opponentState;

    private int spawnCount;
    public int SpawnCount => spawnCount;

    private int deadCount;
    public int DeadCount => deadCount;

    public ChallengeState(
        PlayerState playerState,
        PlayerState opponentState,
        int spawnCount,
        int deadCount
    )
    {
        this.playerState = playerState;
        this.opponentState = opponentState;
        this.spawnCount = spawnCount;
        this.deadCount = deadCount;
    }

    public bool Equals(
        PlayerState playerState,
        PlayerState opponentState,
        int spawnCount,
        int deadCount
    )
    {
        return FirstDiff(
            playerState,
            opponentState,
            spawnCount,
            deadCount
        ) == null;
    }

    /*
     * Returns the first difference between this and other PlayerState instance.
     */
    public string FirstDiff(
        PlayerState playerState,
        PlayerState opponentState,
        int spawnCount,
        int deadCount
    )
    {
        if (!this.playerState.Equals(playerState))
        {
            return this.playerState.FirstDiff(playerState);
        }
        else if (!this.playerState.Equals(opponentState))
        {
            return this.opponentState.FirstDiff(opponentState);
        }
        else if (this.spawnCount != spawnCount)
        {
            return string.Format("SpawnCount: {0} vs {1}", this.spawnCount, spawnCount);
        }
        else if (this.deadCount != deadCount)
        {
            return string.Format("DeadCount: {0} vs {1}", this.deadCount, deadCount);
        }

        return null;
    }
}
