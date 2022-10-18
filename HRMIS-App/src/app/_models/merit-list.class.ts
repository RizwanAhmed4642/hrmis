export class MeritList {
    public values: MeritListValues;

}
export class MeritListValues {
    public topper: any;
    public selected: any[];
    public waiting: any[];
    public stats: MeritListCounts;
}
export class MeritListCounts {
    public matricCounts: number = 0;
    public matricErrors: number = 0;
    public interCounts: number = 0;
    public interErrors: number = 0;
    public gradCounts: number = 0;
    public gradErrors: number = 0;
    public firstHigherCounts: number = 0;
    public secondHigherCounts: number = 0;
    public thirdHigherCounts: number = 0;
    public firstPositionCounts: number = 0;
    public secondPositionCounts: number = 0;
    public thirdPositionCounts: number = 0;
    public oneYearExpCounts: number = 0;
    public twoYearExpCounts: number = 0;
    public threeYearExpCounts: number = 0;
    public fourYearExpCounts: number = 0;
    public fivePlusYearExpCounts: number = 0;
    public hufazCounts: number = 0;
    public oneMarksinterviewCounts: number = 0;
    public twoMarksinterviewCounts: number = 0;
    public threeMarksinterviewCounts: number = 0;
    public fourMarksinterviewCounts: number = 0;
    public fiveMarksinterviewCounts: number = 0;
    public errors: number = 0;
}