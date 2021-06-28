package be.kdg.stemtest.model.Interactors

import be.kdg.stemtest.model.entity.GameSettings
import be.kdg.stemtest.model.entity.Identification
import be.kdg.stemtest.model.rest.RemoteDataSource
import io.reactivex.Single
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers
import javax.inject.Inject

class ConnectionRemoteInteractor @Inject constructor(private val connectionDatabaseInteractor: ConnectionDatabaseInteractor,
                                                     private val remoteDataSource: RemoteDataSource
) {

    fun getIdentification(sessionCode: Int): Single<Identification> {
        return remoteDataSource.getStemTestService().addStudent(sessionCode)
            .map { s -> Identification(sessionCode, s) }
            .doOnSuccess { i -> connectionDatabaseInteractor.saveData(i) }
    }

    fun addStudent(sessionCode: Int): Single<Identification> {
        return remoteDataSource.getStemTestService().addStudent(sessionCode)
            .map { i -> Identification(sessionCode, i) }
            .doOnSuccess { i ->
                if (i.studentId > 0) {
                    connectionDatabaseInteractor.saveData(i)
                        .subscribeOn(Schedulers.io())
                        .observeOn(AndroidSchedulers.mainThread())
                        .subscribe()
                }
            }
    }

    fun getDefinitions(sessionCode: Int) {
        remoteDataSource.getStemTestService().getDefinitions(sessionCode)
        .doOnSuccess { i -> connectionDatabaseInteractor.insertAllDefinitions(i) }
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
            .subscribe()
    }

    fun getSettings(sessionCode: Int): Single<GameSettings> {
       return remoteDataSource.getStemTestService().getSettings(sessionCode)
            .doOnSuccess{ i -> connectionDatabaseInteractor.insertSettings(i)
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe()
            }

    }

    fun getParties(sessionCode: Int) {
        remoteDataSource.getStemTestService().getParties(sessionCode)
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .doOnSuccess {p ->
                connectionDatabaseInteractor.saveParties(p)
            }
            .subscribe()
    }
}