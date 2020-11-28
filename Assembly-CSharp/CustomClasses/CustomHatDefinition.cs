using System;

// Token: 0x020002CF RID: 719
public class CustomHatDefinition
{
	// Token: 0x1700021C RID: 540
	// (get) Token: 0x06000ED7 RID: 3799 RVA: 0x0000AA3B File Offset: 0x00008C3B
	// (set) Token: 0x06000ED8 RID: 3800 RVA: 0x0000AA43 File Offset: 0x00008C43
	public string ID { get; set; } = "NullHat";

	// Token: 0x1700021D RID: 541
	// (get) Token: 0x06000ED9 RID: 3801 RVA: 0x0000AA4C File Offset: 0x00008C4C
	// (set) Token: 0x06000EDA RID: 3802 RVA: 0x0000AA54 File Offset: 0x00008C54
	public string NormalImg { get; set; } = string.Empty;

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x06000EDB RID: 3803 RVA: 0x0000AA5D File Offset: 0x00008C5D
	// (set) Token: 0x06000EDC RID: 3804 RVA: 0x0000AA65 File Offset: 0x00008C65
	public string FloorImg { get; set; } = string.Empty;

	// Token: 0x1700021F RID: 543
	// (get) Token: 0x06000EDD RID: 3805 RVA: 0x0000AA6E File Offset: 0x00008C6E
	// (set) Token: 0x06000EDE RID: 3806 RVA: 0x0000AA76 File Offset: 0x00008C76
	public bool inFront { get; set; }

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x06000EDF RID: 3807 RVA: 0x0000AA7F File Offset: 0x00008C7F
	// (set) Token: 0x06000EE0 RID: 3808 RVA: 0x0000AA87 File Offset: 0x00008C87
	public float NormalPosX { get; set; }

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x06000EE1 RID: 3809 RVA: 0x0000AA90 File Offset: 0x00008C90
	// (set) Token: 0x06000EE2 RID: 3810 RVA: 0x0000AA98 File Offset: 0x00008C98
	public float NormalPosY { get; set; }

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x06000EE3 RID: 3811 RVA: 0x0000AAA1 File Offset: 0x00008CA1
	// (set) Token: 0x06000EE4 RID: 3812 RVA: 0x0000AAA9 File Offset: 0x00008CA9
	public float NormalWidth { get; set; }

	// Token: 0x17000223 RID: 547
	// (get) Token: 0x06000EE5 RID: 3813 RVA: 0x0000AAB2 File Offset: 0x00008CB2
	// (set) Token: 0x06000EE6 RID: 3814 RVA: 0x0000AABA File Offset: 0x00008CBA
	public float NormalHeight { get; set; }

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x06000EE7 RID: 3815 RVA: 0x0000AAC3 File Offset: 0x00008CC3
	// (set) Token: 0x06000EE8 RID: 3816 RVA: 0x0000AACB File Offset: 0x00008CCB
	public float NormalPivotX { get; set; }

	// Token: 0x17000225 RID: 549
	// (get) Token: 0x06000EE9 RID: 3817 RVA: 0x0000AAD4 File Offset: 0x00008CD4
	// (set) Token: 0x06000EEA RID: 3818 RVA: 0x0000AADC File Offset: 0x00008CDC
	public float NormalPivotY { get; set; }

	// Token: 0x17000226 RID: 550
	// (get) Token: 0x06000EEB RID: 3819 RVA: 0x0000AAE5 File Offset: 0x00008CE5
	// (set) Token: 0x06000EEC RID: 3820 RVA: 0x0000AAED File Offset: 0x00008CED
	public float FloorPosX { get; set; }

	// Token: 0x17000227 RID: 551
	// (get) Token: 0x06000EED RID: 3821 RVA: 0x0000AAF6 File Offset: 0x00008CF6
	// (set) Token: 0x06000EEE RID: 3822 RVA: 0x0000AAFE File Offset: 0x00008CFE
	public float FloorPosY { get; set; }

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x06000EEF RID: 3823 RVA: 0x0000AB07 File Offset: 0x00008D07
	// (set) Token: 0x06000EF0 RID: 3824 RVA: 0x0000AB0F File Offset: 0x00008D0F
	public float FloorWidth { get; set; }

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x06000EF1 RID: 3825 RVA: 0x0000AB18 File Offset: 0x00008D18
	// (set) Token: 0x06000EF2 RID: 3826 RVA: 0x0000AB20 File Offset: 0x00008D20
	public float FloorHeight { get; set; }

	// Token: 0x1700022A RID: 554
	// (get) Token: 0x06000EF3 RID: 3827 RVA: 0x0000AB29 File Offset: 0x00008D29
	// (set) Token: 0x06000EF4 RID: 3828 RVA: 0x0000AB31 File Offset: 0x00008D31
	public float FloorPivotX { get; set; }

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x06000EF5 RID: 3829 RVA: 0x0000AB3A File Offset: 0x00008D3A
	// (set) Token: 0x06000EF6 RID: 3830 RVA: 0x0000AB42 File Offset: 0x00008D42
	public float FloorPivotY { get; set; }

	public bool UsePointFiltering { get; set; } = false;
}
