import { callExternalApi } from "../apiHelper";

const apiServerUrl = `${process.env.REACT_APP_API_BASE_URL}/v1/Bookmarks`;

export const getAllBookmarks = async(accessToken, showHidden = false) => {
    const config = {
        url: `${apiServerUrl}?showHidden=${showHidden}`,
        method: "GET",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        },
    };

    return await callExternalApi({config});
};

export const getBookmark = async(accessToken, id) => {
    const config = {
        url: `${apiServerUrl}/${id}`,
        method: "GET",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        },
    };

    return await callExternalApi({config});
};

export const createNewBookmark = async(accessToken, newBookmark) => {
    const config = {
        url: `${apiServerUrl}`,
        method: "POST",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        },
        data: newBookmark
    };

    return await callExternalApi({config});
};

export const updateBookmark = async(accessToken, id, updatedBookmarkEntry) => {
    const config = {
        url: `${apiServerUrl}/${id}`,
        method: "PUT",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        },
        data: updatedBookmarkEntry
    };

    return await callExternalApi({config});
};

export const addNewBookmarkTags = async(accessToken, id, tags) => {
    const config = {
        url: `${apiServerUrl}/${id}/Tags`,
        method: "POST",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        },
        data: {
            tags: tags
        }
    };

    return await callExternalApi({config});
};

export const deleteBookmark = async(accessToken, id) => {
    const config = {
        url: `${apiServerUrl}/${id}`,
        method: "DELETE",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        }
    };

    return await callExternalApi({config});
};

export const deleteBookmarks = async(accessToken, ids) => {
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

export const hideBookmarks = async(accessToken, ids) => {
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