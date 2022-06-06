const CHART_COLORS = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)',
    brown: 'rgb(128,64,0)',
    pink: 'rgb(255,128,192)',
    violet: 'rgb(128,0,255)',
    khaki: 'rgb(128,128,64)'
};

const NAMED_COLORS = [
    CHART_COLORS.red,
    CHART_COLORS.orange,
    CHART_COLORS.yellow,
    CHART_COLORS.green,
    CHART_COLORS.blue,
    CHART_COLORS.purple,
    CHART_COLORS.grey,
    CHART_COLORS.brown,
    CHART_COLORS.pink,
    CHART_COLORS.violet,
    CHART_COLORS.khaki,
];

function namedColor(index) {
    return NAMED_COLORS[index % NAMED_COLORS.length];
}