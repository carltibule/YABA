import { callExternalApi } from "../apiHelper";

const apiServerUrl = `${process.env.REACT_APP_API_BASE_URL}/v1/Tags`;

export const getAllTags = async(accessToken) => {
    const config = {
        url: `${apiServerUrl}`,
        method: "GET",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        },
    };

    return await callExternalApi({config});
};

export const createNewTag = async(accessToken, newTag) => {
    const config = {
        url: `${apiServerUrl}`,
        method: "POST",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        },
        data: newTag
    };

    return await callExternalApi({config});
};

export const updateTag = async(accessToken, id, updatedTagEntry) => {
    const config = {
        url: `${apiServerUrl}/${id}`,
        method: "PUT",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        },
        data: updatedTagEntry
    };

    return await callExternalApi({config});
};

export const deleteTags = async(accessToken, ids) => {
    const config = {
        url: `${apiServerUrl}`,
        method: "DELETE",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        },
        data: {
            ids: ids
        }
    };

    return await callExternalApi({config});
};

export const hideTags = async(accessToken, ids) => {
    const config = {
        url: `${apiServerUrl}/Hide`,
        method: "POST",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        },
        data: {
            ids: ids
        }
    };

    return await callExternalApi({config});
};