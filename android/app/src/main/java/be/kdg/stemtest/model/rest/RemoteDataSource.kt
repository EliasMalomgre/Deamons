package be.kdg.stemtest.model.rest

import retrofit2.Retrofit
import retrofit2.adapter.rxjava2.RxJava2CallAdapterFactory
import retrofit2.converter.gson.GsonConverterFactory
import javax.inject.Inject


class RemoteDataSource @Inject constructor(private val retrofit: Retrofit){

    fun getStemTestService(): StemTestService {
        return retrofit.create(StemTestService::class.java)
    }
}