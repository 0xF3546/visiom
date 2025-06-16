// generated with @7nohe/openapi-react-query-codegen@1.6.2 

import { type QueryClient } from "@tanstack/react-query";
import { SearchService } from "../requests/services.gen";
import * as Common from "./common";
export const prefetchUseSearchServiceGetApiSearch = (queryClient: QueryClient, { nextPageToken, page, pageSize, q }: {
  nextPageToken?: string;
  page?: number;
  pageSize?: number;
  q?: string;
} = {}) => queryClient.prefetchQuery({ queryKey: Common.UseSearchServiceGetApiSearchKeyFn({ nextPageToken, page, pageSize, q }), queryFn: () => SearchService.getApiSearch({ nextPageToken, page, pageSize, q }) });
