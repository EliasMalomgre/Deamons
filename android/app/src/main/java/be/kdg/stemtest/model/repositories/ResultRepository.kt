package be.kdg.stemtest.model.repositories

import android.util.Log
import androidx.lifecycle.LiveData
import androidx.lifecycle.LiveDataReactiveStreams
import be.kdg.stemtest.model.Interactors.ResultDatabaseInteractor
import be.kdg.stemtest.model.Interactors.ResultRemoteInteractor
import be.kdg.stemtest.model.entity.*
import io.reactivex.BackpressureStrategy
import io.reactivex.Observable
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers
import java.util.concurrent.TimeUnit
import javax.inject.Inject

class ResultRepository @Inject constructor(private val databaseInteractor:ResultDatabaseInteractor,
                                           private val remoteInteractor:ResultRemoteInteractor)  {
    fun getPartyGameResult(): LiveData<String> {
        val database = databaseInteractor.getPartyGameResult().toObservable()
        val remote = remoteInteractor.getPartyGameResult().toObservable()

        val observable = Observable.concat(database,remote)
            .onErrorReturn{ "error" }
            .retryWhen { i -> i.delay(4,TimeUnit.SECONDS)  }
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
        return LiveDataReactiveStreams.fromPublisher(observable.toFlowable(BackpressureStrategy.ERROR))
    }

    fun getDebateGameResult(): LiveData<List<DebateGameResult>> {
        val database = databaseInteractor.getDebateResult()
            .toObservable()
        val remote = remoteInteractor.getDebateResult()
            .toObservable()

        val observable = Observable.concat(database,remote)
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
        return LiveDataReactiveStreams.fromPublisher(observable.toFlowable(BackpressureStrategy.ERROR))
    }

    fun getAnswers(): LiveData<List<StudentAnswer>> {
        val database = databaseInteractor.getAnswers()
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
        return LiveDataReactiveStreams.fromPublisher(database.toFlowable())
    }

    fun getPartyAnswers(): LiveData<List<PartyAnswer>> {
        val database = databaseInteractor.getPartyAnswers().toObservable()
        val remote = remoteInteractor.getPartyAnswers().toObservable()
        val observable = Observable.concat(database,remote)
            .onErrorReturn { listOf(PartyAnswer(-1,"error",-1,-1,"error")) }
            .retryWhen { i -> i.delay(4,TimeUnit.SECONDS)  }
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())

        return LiveDataReactiveStreams.fromPublisher(observable.toFlowable(BackpressureStrategy.ERROR))
    }

    fun getCustomPartyGameResult(): LiveData<String> {
        val database = databaseInteractor.getPartyGameResult().toObservable()
        val remote = remoteInteractor.getCustomPartyGameResult().toObservable()

        val observable = Observable.concat(database,remote)
            .onErrorReturn { "error" }
            .retryWhen { i -> i.delay(4,TimeUnit.SECONDS)  }
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())

        return LiveDataReactiveStreams.fromPublisher(observable.toFlowable(BackpressureStrategy.ERROR))
    }

    fun getCorrectAnswers() : LiveData<List<AnswerOption>> {

        val database = databaseInteractor.getCorrectAnswers().toObservable()
        val remote = remoteInteractor.getCorrectAnswers().toObservable()
        val observable = Observable.concat(database,remote)
            .onErrorReturn { listOf(AnswerOption(-1,-1,"error")) }
            .retryWhen { i -> i.delay(4,TimeUnit.SECONDS)  }
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())

        return LiveDataReactiveStreams.fromPublisher(observable.toFlowable(BackpressureStrategy.ERROR))
    }

    fun getAllPartyAnswers(): LiveData<List<PartyAnswer>> {
        val database = databaseInteractor.getPartyAnswers().toObservable()
        val remote = remoteInteractor.getAllPartyAnswers().toObservable()
        val observable = Observable.concat(database,remote)
            .onErrorReturn { listOf(PartyAnswer(-1,"error",-1,-1,"error")) }
            .retryWhen { i -> i.delay(4,TimeUnit.SECONDS)  }
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
        return LiveDataReactiveStreams.fromPublisher(observable.toFlowable(BackpressureStrategy.ERROR))
    }

    fun getCustomDebateGameResult(index :Int): LiveData<CustomDebateGameResult> {
        val database = databaseInteractor.getCustomDebateGameResult(index).toObservable()
        val remote = remoteInteractor.getCustomDebateGameResults(index).toObservable()

        val observable = Observable.concat(database,remote)
            .onErrorReturn { CustomDebateGameResult(-1, floatArrayOf()) }
            .retryWhen { i -> i.delay(4,TimeUnit.SECONDS)  }
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())

        return LiveDataReactiveStreams.fromPublisher(observable.toFlowable(BackpressureStrategy.ERROR))
    }
}