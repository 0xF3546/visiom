// generated with @7nohe/openapi-react-query-codegen@1.6.2 

import { UseQueryResult } from "@tanstack/react-query";
import { SearchService } from "../requests/services.gen";
export type SearchServiceGetApiSearchDefaultResponse = Awaited<ReturnType<typeof SearchService.getApiSearch>>;
export type SearchServiceGetApiSearchQueryResult<TData = SearchServiceGetApiSearchDefaultResponse, TError = unknown> = UseQueryResult<TData, TError>;
export const useSearchServiceGetApiSearchKey = "SearchServiceGetApiSearch";
export const UseSearchServiceGetApiSearchKeyFn = ({ nextPageToken, page, pageSize, q }: {
  nextPageToken?: string;
  page?: number;
  pageSize?: number;
  q?: string;
} = {}, queryKey?: Array<unknown>) => [useSearchServiceGetApiSearchKey, ...(queryKey ?? [{ nextPageToken, page, pageSize, q }])];
