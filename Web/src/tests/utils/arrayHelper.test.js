import { isSubset } from "../../utils/arrayHelper";

describe("utils/arrayHelper", () => {
    describe("isSubset", () => {
        const testCases = [
            {
                parentArray: ["test1", "test2", "test3", "test4"],
                subsetArray: ["test1"],
                expected: true
            },
            {
                parentArray: ["test1", "test2", "test3", "test4"],
                subsetArray: ["test5"],
                expected: false
            },
            {
                parentArray: ["test1", "test2", "test3", "test4"],
                subsetArray: ["test2", "test4"],
                expected: true
            }
        ];

        testCases.forEach(test => {
            it(`[${test.subsetArray.join(", ")}] should be a subset of [${test.parentArray.join(", ")}] which is ${test.expected}`, () => {
                const actual = isSubset(test.parentArray, test.subsetArray);
                expect(actual).toEqual(test.expected);
            });
        });
    });
});