using System;

public sealed class Lab : Component, IDisposable
{
    [Property, Feature("Props")] public GameObject Replicant { get; set; }
    [Property, Feature("Props")] public GameObject Spawn { get; set; }

    [Property, Feature("Buttons")] public UseBox ButtonReplicate { get; set; }
    [Property, Feature("Buttons")] public UseBox ButtonBuy { get; set; }
    [Property, Feature("Buttons")] public UseBox ButtonHeal { get; set; }
    [Property, Feature("Buttons")] public UseBox ButtonBodyHead { get; set; }
    [Property, Feature("Buttons")] public UseBox ButtonBodyLeftHand { get; set; }
    [Property, Feature("Buttons")] public UseBox ButtonBodyRightHand { get; set; }
    [Property, Feature("Buttons")] public UseBox ButtonBodyLeftLeg { get; set; }
    [Property, Feature("Buttons")] public UseBox ButtonBodyRightLeg { get; set; }
    [Property, Feature("Buttons")] public List<UseBox> ButtonsAbilities { get; set; } = new();

    [Property, Feature("Prefabs")] public GameObject ReplicantPrefab { get; set; }

    public void Dispose()
    {
        //Unscribe();
        //? he auto collect OnCallbackes and unscribe them
    }

    private void Prepare()
    {
        if (!CheckGameObjects())
        {
            Log.Error($"[Lab] some object not selected! ({GameObject})");

            return;
        }

        Subscribe();
        Sync();
    }

    private void Sync()
    {
        var info = LabInfo.Instance;

        SetupBodygroups();
    }

    private void SetupBodygroups(int head = 0, int lHand = 0, int rHand = 0, int lLeg = 0, int rLeg = 0)
    {
        if (!Replicant.IsValid()) return;

        var model = Replicant.GetComponent<ModelRenderer>();
        if (!model.IsValid()) return;

        model.SetBodyGroup("head", head);
        model.SetBodyGroup("l_hand", lHand);
        model.SetBodyGroup("r_hand", rHand);
        model.SetBodyGroup("l_leg", lLeg);
        model.SetBodyGroup("r_leg", rLeg);
    }

    private void Subscribe()
    {
        ButtonReplicate.OnCallback += BuyReplicate;
        ButtonHeal.OnCallback += BuyHeal;
        //start work
    }

    private void Unscribe()
    {
        ButtonReplicate.OnCallback -= BuyReplicate;
        ButtonHeal.OnCallback -= BuyHeal;
    }

    private bool CheckGameObjects()
    {
        var a = ButtonReplicate.IsValid();
        var b = ButtonHeal.IsValid();
        var c = (ButtonsAbilities.Count > 0);
        var d = ReplicantPrefab.IsValid();
        var bh = ButtonBodyHead.IsValid();
        var blh = ButtonBodyLeftHand.IsValid();
        var brh = ButtonBodyRightHand.IsValid();
        var bll = ButtonBodyLeftLeg.IsValid();
        var brl = ButtonBodyRightLeg.IsValid();

        return (a && b && c && d && bh && blh && brh && bll && brl);
    }

    private void BuyReplicate()
    {
        var ply = Player.Instance;
        if (ply.ReplicantController.GetCountReplicants() >= GlobalSettings.MaxReplicants)
        {
            //todo Player.Error
            //todo sound.Error

            return;
        }

        var dna = ply.Dna;
        var cost = GlobalSettings.CostReplicate;

        if (!CanBuy(cost))
        {
            //todo Player.Error
            //todo sound.Error

            return;
        }

        ply.Dna -= cost;

        SpawnReplicator();
    }

    private void SpawnReplicator()
    {
        var obj = ReplicantPrefab.Clone(Spawn.WorldPosition, Rotation.Identity);
        var repli = obj.GetComponent<Replicant>();

        //todo sound
        //todo particle

        Player.Instance.ReplicantController.AddReplicant(repli);
    }

    private void BuyHeal()
    {
        var ply = Player.Instance;
        if (ply.ReplicantController.GetCountReplicants() == 0)
        {
            //todo Player.Error
            //todo sound.Error

            return;
        }

        var dna = ply.Dna;
        var cost = GlobalSettings.CostHeal;

        if (!CanBuy(cost))
        {
            //todo Player.Error
            //todo sound.Error

            return;
        }

        ply.Dna -= cost;

        HealReplicators();
    }

    private void HealReplicators()
    {
        foreach (var replicant in Player.Instance.ReplicantController.Replicants)
        {
            replicant.Health = replicant.MaxHealth;
        }
    }
    
    private bool CanBuy(int cost)
    {
        return (Player.Instance.Dna >= cost);
    }

    protected override void OnStart()
    {
        Prepare();
    }

    protected override void OnDestroy()
    {
        Dispose();
    }

    //protected override void OnFixedUpdate()
    //{

    //}
}
