// FROM: https://fjolt.com/article/javascript-check-if-array-is-subset
export const isSubset  = (parentArray, subsetArray) => {
    return subsetArray.every((el) => {
        return parentArray.includes(el)
    })
};
