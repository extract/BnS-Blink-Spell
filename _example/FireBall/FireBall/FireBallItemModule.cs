using BS;

namespace FireBall
{
    class FireBallItemModule : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            // This could've been done in the spell script. However, that could cause problems if my assumption on it's spawning is wrong.
            //item.gameObject.AddComponent<FireBallScript>();
        }
    }
}
